using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.SecretsManager;
using Constructs;
using Infra.Config;

namespace Infra.Stacks;

public class DatabaseStackProps : StackProps
{
    public required IVpc Vpc { get; set; }
    public required ISecurityGroup DatabaseSecurityGroup { get; set; }
}

public class DatabaseStack : Stack
{
    public ISecret DatabaseSecret { get; }
    public string DatabaseEndpoint { get; }
    public string DatabasePort { get; }

    public DatabaseStack(Construct scope, string id, DatabaseStackProps props) : base(scope, id, props)
    {
        var config = new EnvironmentConfig
        {
            AccountId = props.Env?.Account ?? "",
            Region = props.Env?.Region ?? "",
            EnvironmentName = "dev",
            ProjectName = "orchard-modernization-poc"
        };

        // Secrets Manager secret for database credentials
        DatabaseSecret = new Secret(this, "DbSecret", new SecretProps
        {
            SecretName = $"{config.ProjectName}/database/{config.EnvironmentName}",
            Description = "RDS SQL Server credentials for Orchard CMS POC",
            GenerateSecretString = new SecretStringGenerator
            {
                SecretStringTemplate = $"{{\"username\":\"{config.DatabaseAdminUsername}\"}}",
                GenerateStringKey = "password",
                ExcludePunctuation = true,
                PasswordLength = 32
            }
        });

        // RDS SQL Server Standard instance
        var dbInstance = new DatabaseInstance(this, "RdsInstance", new DatabaseInstanceProps
        {
            InstanceIdentifier = $"{config.ProjectName}-sqlserver-{config.EnvironmentName}",
            Engine = DatabaseInstanceEngine.SqlServerSe(new SqlServerSeInstanceEngineProps
            {
                Version = SqlServerEngineVersion.VER_15
            }),
            InstanceType = InstanceType.Of(InstanceClass.M5, InstanceSize.LARGE),
            Vpc = props.Vpc,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_ISOLATED },
            SecurityGroups = new ISecurityGroup[] { props.DatabaseSecurityGroup },
            Credentials = Credentials.FromSecret(DatabaseSecret),
            MultiAz = config.DatabaseMultiAz,
            AllocatedStorage = config.DatabaseAllocatedStorageGb,
            StorageType = StorageType.GP3,
            DeletionProtection = false, // POC only - set true for production
            RemovalPolicy = RemovalPolicy.DESTROY, // POC only - set RETAIN for production
            BackupRetention = Duration.Days(7),
            PreferredBackupWindow = "03:00-04:00",
            PreferredMaintenanceWindow = "sun:04:00-sun:05:00",
            AutoMinorVersionUpgrade = true,
            PubliclyAccessible = false
        });

        DatabaseEndpoint = dbInstance.DbInstanceEndpointAddress;
        DatabasePort = dbInstance.DbInstanceEndpointPort;

        // Outputs
        _ = new CfnOutput(this, "DbEndpoint", new CfnOutputProps
        {
            Value = DatabaseEndpoint,
            ExportName = $"{config.ProjectName}-db-endpoint"
        });
        _ = new CfnOutput(this, "DbSecretArn", new CfnOutputProps
        {
            Value = DatabaseSecret.SecretArn,
            ExportName = $"{config.ProjectName}-db-secret-arn"
        });
    }
}
