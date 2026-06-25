using Amazon.CDK;
using Infra.Config;
using Infra.Stacks;

var app = new App();

var envConfig = new EnvironmentConfig
{
    AccountId = "842387632939",
    Region = "eu-west-2",
    EnvironmentName = "dev",
    ProjectName = "orchard-modernization-poc"
};

var env = new Amazon.CDK.Environment
{
    Account = envConfig.AccountId,
    Region = envConfig.Region
};

var networkStack = new NetworkStack(app, $"{envConfig.ProjectName}-network-{envConfig.EnvironmentName}", new StackProps
{
    Env = env,
    Description = "Orchard CMS POC - VPC, subnets, NAT, security groups"
});

var databaseStack = new DatabaseStack(app, $"{envConfig.ProjectName}-database-{envConfig.EnvironmentName}", new DatabaseStackProps
{
    Env = env,
    Description = "Orchard CMS POC - RDS SQL Server Standard with Secrets Manager",
    Vpc = networkStack.Vpc,
    DatabaseSecurityGroup = networkStack.DatabaseSecurityGroup
});

var computeStack = new ComputeStack(app, $"{envConfig.ProjectName}-compute-{envConfig.EnvironmentName}", new ComputeStackProps
{
    Env = env,
    Description = "Orchard CMS POC - ECS on EC2 (Windows), ALB, ECR",
    Vpc = networkStack.Vpc,
    EcsSecurityGroup = networkStack.EcsSecurityGroup,
    AlbSecurityGroup = networkStack.AlbSecurityGroup,
    DatabaseSecret = databaseStack.DatabaseSecret,
    DatabaseEndpoint = databaseStack.DatabaseEndpoint,
    DatabasePort = databaseStack.DatabasePort
});

var monitoringStack = new MonitoringStack(app, $"{envConfig.ProjectName}-monitoring-{envConfig.EnvironmentName}", new MonitoringStackProps
{
    Env = env,
    Description = "Orchard CMS POC - CloudWatch log group, CPU/memory/5xx alarms",
    EcsCluster = computeStack.EcsCluster,
    EcsService = computeStack.EcsService,
    LoadBalancer = computeStack.LoadBalancer
});

Infra.Config.Tags.ApplyStandardTags(app, envConfig);

app.Synth();
