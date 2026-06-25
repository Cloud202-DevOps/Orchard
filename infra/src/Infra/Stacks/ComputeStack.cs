using Amazon.CDK;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SecretsManager;
using Constructs;
using Infra.Config;
using HealthCheck = Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck;

namespace Infra.Stacks;

public class ComputeStackProps : StackProps
{
    public required IVpc Vpc { get; set; }
    public required ISecurityGroup EcsSecurityGroup { get; set; }
    public required ISecurityGroup AlbSecurityGroup { get; set; }
    public required ISecret DatabaseSecret { get; set; }
    public required string DatabaseEndpoint { get; set; }
    public required string DatabasePort { get; set; }
}

public class ComputeStack : Stack
{
    public ICluster EcsCluster { get; }
    public IBaseService EcsService { get; }
    public IApplicationLoadBalancer LoadBalancer { get; }

    public ComputeStack(Construct scope, string id, ComputeStackProps props) : base(scope, id, props)
    {
        var config = new EnvironmentConfig
        {
            AccountId = props.Env?.Account ?? "",
            Region = props.Env?.Region ?? "",
            EnvironmentName = "dev",
            ProjectName = "orchard-modernization-poc"
        };

        // ECR Repository
        var repository = new Repository(this, "EcrRepo", new RepositoryProps
        {
            RepositoryName = $"{config.ProjectName}",
            RemovalPolicy = RemovalPolicy.DESTROY,
            EmptyOnDelete = true,
            LifecycleRules = new ILifecycleRule[]
            {
                new LifecycleRule
                {
                    MaxImageCount = 10,
                    Description = "Keep only 10 most recent images"
                }
            }
        });

        // ECS Cluster
        var cluster = new Cluster(this, "EcsCluster", new ClusterProps
        {
            ClusterName = $"{config.ProjectName}-cluster-{config.EnvironmentName}",
            Vpc = props.Vpc,
            ContainerInsights = true
        });
        EcsCluster = cluster;

        // Auto Scaling Group for ECS EC2 capacity (Windows)
        var asg = new AutoScalingGroup(this, "EcsAsg", new AutoScalingGroupProps
        {
            Vpc = props.Vpc,
            InstanceType = InstanceType.Of(InstanceClass.M5, InstanceSize.XLARGE),
            MachineImage = EcsOptimizedImage.Windows(WindowsOptimizedVersion.SERVER_2022),
            MinCapacity = config.EcsMinCapacity,
            MaxCapacity = config.EcsMaxCapacity,
            DesiredCapacity = config.EcsDesiredCapacity,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
            SecurityGroup = props.EcsSecurityGroup,
            NewInstancesProtectedFromScaleIn = false
        });

        // Add capacity provider
        var capacityProvider = new AsgCapacityProvider(this, "AsgCapacityProvider", new AsgCapacityProviderProps
        {
            AutoScalingGroup = asg,
            CapacityProviderName = $"{config.ProjectName}-cp-{config.EnvironmentName}",
            EnableManagedScaling = true,
            EnableManagedTerminationProtection = false,
            TargetCapacityPercent = 100
        });
        cluster.AddAsgCapacityProvider(capacityProvider);

        // CloudWatch Log Group for ECS tasks
        var logGroup = new LogGroup(this, "TaskLogGroup", new LogGroupProps
        {
            LogGroupName = $"/ecs/{config.ProjectName}/{config.EnvironmentName}",
            Retention = RetentionDays.THIRTY_DAYS,
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        // Task Definition (Windows)
        var taskDefinition = new Ec2TaskDefinition(this, "TaskDef", new Ec2TaskDefinitionProps
        {
            NetworkMode = NetworkMode.NAT,
            Family = $"{config.ProjectName}-task-{config.EnvironmentName}"
        });

        // Grant task role access to Secrets Manager
        props.DatabaseSecret.GrantRead(taskDefinition.TaskRole);

        // Container Definition
        var container = taskDefinition.AddContainer("OrchardWeb", new ContainerDefinitionOptions
        {
            Image = ContainerImage.FromEcrRepository(repository, "latest"),
            MemoryLimitMiB = config.TaskMemoryMiB,
            Cpu = config.TaskCpu,
            Essential = true,
            Logging = LogDriver.AwsLogs(new AwsLogDriverProps
            {
                LogGroup = logGroup,
                StreamPrefix = "orchard"
            }),
            Environment = new Dictionary<string, string>
            {
                ["ORCHARD_DB_SERVER"] = props.DatabaseEndpoint,
                ["ORCHARD_DB_PORT"] = props.DatabasePort,
                ["ORCHARD_ENVIRONMENT"] = config.EnvironmentName
            },
            Secrets = new Dictionary<string, Secret>
            {
                ["ORCHARD_DB_SECRET_ARN"] = Amazon.CDK.AWS.ECS.Secret.FromSecretsManager(props.DatabaseSecret)
            }
        });

        container.AddPortMappings(new PortMapping
        {
            ContainerPort = config.ContainerPort,
            HostPort = 80,
            Protocol = Amazon.CDK.AWS.ECS.Protocol.TCP
        });

        // Application Load Balancer
        LoadBalancer = new ApplicationLoadBalancer(this, "Alb", new ApplicationLoadBalancerProps
        {
            Vpc = props.Vpc,
            InternetFacing = true,
            SecurityGroup = props.AlbSecurityGroup,
            LoadBalancerName = $"{config.ProjectName}-alb-{config.EnvironmentName}",
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PUBLIC }
        });

        var listener = LoadBalancer.AddListener("HttpListener", new BaseApplicationListenerProps
        {
            Port = 80,
            Protocol = ApplicationProtocol.HTTP
        });

        // ECS Service
        var service = new Ec2Service(this, "EcsService", new Ec2ServiceProps
        {
            Cluster = cluster,
            TaskDefinition = taskDefinition,
            DesiredCount = 1,
            ServiceName = $"{config.ProjectName}-service-{config.EnvironmentName}",
            CapacityProviderStrategies = new ICapacityProviderStrategy[]
            {
                new CapacityProviderStrategy
                {
                    CapacityProvider = capacityProvider.CapacityProviderName,
                    Weight = 1
                }
            },
            CircuitBreaker = new DeploymentCircuitBreaker
            {
                Enable = true,
                Rollback = true
            },
            HealthCheckGracePeriod = Duration.Seconds(config.HealthCheckGracePeriodSeconds)
        });
        EcsService = service;

        // Register ECS service with ALB target group
        listener.AddTargets("EcsTargets", new AddApplicationTargetsProps
        {
            Port = 80,
            Targets = new IApplicationLoadBalancerTarget[] { service },
            HealthCheck = new HealthCheck
            {
                Path = config.AlbHealthCheckPath,
                Interval = Duration.Seconds(config.AlbHealthCheckIntervalSeconds),
                HealthyThresholdCount = config.AlbHealthyThresholdCount,
                UnhealthyThresholdCount = config.AlbUnhealthyThresholdCount,
                Timeout = Duration.Seconds(10)
            },
            DeregistrationDelay = Duration.Seconds(30)
        });

        // Outputs
        _ = new CfnOutput(this, "AlbDns", new CfnOutputProps
        {
            Value = LoadBalancer.LoadBalancerDnsName,
            ExportName = $"{config.ProjectName}-alb-dns"
        });
        _ = new CfnOutput(this, "EcrRepoUri", new CfnOutputProps
        {
            Value = repository.RepositoryUri,
            ExportName = $"{config.ProjectName}-ecr-uri"
        });
        _ = new CfnOutput(this, "ClusterName", new CfnOutputProps
        {
            Value = cluster.ClusterName,
            ExportName = $"{config.ProjectName}-cluster-name"
        });
        _ = new CfnOutput(this, "ServiceName", new CfnOutputProps
        {
            Value = service.ServiceName,
            ExportName = $"{config.ProjectName}-service-name"
        });
    }
}
