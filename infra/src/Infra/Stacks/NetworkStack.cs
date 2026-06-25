using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Constructs;
using Infra.Config;

namespace Infra.Stacks;

public class NetworkStack : Stack
{
    public IVpc Vpc { get; }
    public ISecurityGroup EcsSecurityGroup { get; }
    public ISecurityGroup AlbSecurityGroup { get; }
    public ISecurityGroup DatabaseSecurityGroup { get; }

    public NetworkStack(Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
    {
        var config = new EnvironmentConfig
        {
            AccountId = props?.Env?.Account ?? "",
            Region = props?.Env?.Region ?? "",
            EnvironmentName = "dev",
            ProjectName = "orchard-modernization-poc"
        };

        // VPC with public and private subnets across 2 AZs
        Vpc = new Vpc(this, "Vpc", new VpcProps
        {
            VpcName = $"{config.ProjectName}-vpc-{config.EnvironmentName}",
            IpAddresses = IpAddresses.Cidr(config.VpcCidr),
            MaxAzs = config.MaxAzs,
            NatGateways = 1,
            SubnetConfiguration = new ISubnetConfiguration[]
            {
                new SubnetConfiguration
                {
                    Name = "Public",
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24
                },
                new SubnetConfiguration
                {
                    Name = "Private",
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS,
                    CidrMask = 24
                },
                new SubnetConfiguration
                {
                    Name = "Isolated",
                    SubnetType = SubnetType.PRIVATE_ISOLATED,
                    CidrMask = 24
                }
            }
        });

        // ALB Security Group - allows inbound HTTP/HTTPS from internet
        AlbSecurityGroup = new SecurityGroup(this, "AlbSg", new SecurityGroupProps
        {
            Vpc = Vpc,
            SecurityGroupName = $"{config.ProjectName}-alb-sg-{config.EnvironmentName}",
            Description = "ALB security group - allows HTTP/HTTPS from internet",
            AllowAllOutbound = true
        });
        AlbSecurityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(80), "Allow HTTP from internet");
        AlbSecurityGroup.AddIngressRule(Peer.AnyIpv4(), Port.Tcp(443), "Allow HTTPS from internet");

        // ECS Security Group - allows inbound from ALB only
        EcsSecurityGroup = new SecurityGroup(this, "EcsSg", new SecurityGroupProps
        {
            Vpc = Vpc,
            SecurityGroupName = $"{config.ProjectName}-ecs-sg-{config.EnvironmentName}",
            Description = "ECS tasks security group - allows traffic from ALB only",
            AllowAllOutbound = true
        });
        EcsSecurityGroup.AddIngressRule(AlbSecurityGroup, Port.Tcp(80), "Allow HTTP from ALB");

        // Database Security Group - allows inbound from ECS only
        DatabaseSecurityGroup = new SecurityGroup(this, "DbSg", new SecurityGroupProps
        {
            Vpc = Vpc,
            SecurityGroupName = $"{config.ProjectName}-db-sg-{config.EnvironmentName}",
            Description = "RDS security group - allows SQL Server from ECS only",
            AllowAllOutbound = false
        });
        DatabaseSecurityGroup.AddIngressRule(EcsSecurityGroup, Port.Tcp(1433), "Allow SQL Server from ECS");

        // Outputs
        _ = new CfnOutput(this, "VpcId", new CfnOutputProps { Value = Vpc.VpcId, ExportName = $"{config.ProjectName}-vpc-id" });
    }
}
