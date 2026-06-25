namespace Infra.Config;

public class EnvironmentConfig
{
    public required string AccountId { get; set; }
    public required string Region { get; set; }
    public required string EnvironmentName { get; set; }
    public required string ProjectName { get; set; }

    public string VpcCidr => "10.100.0.0/16";
    public int MaxAzs => 2;

    // RDS Configuration
    public string DatabaseInstanceType => "m5.large";
    public string DatabaseEngine => "sqlserver-se"; // SQL Server Standard
    public string DatabaseEngineVersion => "15.00"; // SQL Server 2019
    public bool DatabaseMultiAz => true;
    public int DatabaseAllocatedStorageGb => 100;
    public string DatabaseName => "OrchardCms";
    public string DatabaseAdminUsername => "orchardadmin";

    // ECS Configuration
    public string EcsInstanceType => "m5.xlarge";
    public int EcsDesiredCapacity => 1;
    public int EcsMinCapacity => 1;
    public int EcsMaxCapacity => 3;
    public int TaskCpu => 2048;
    public int TaskMemoryMiB => 8192;
    public int ContainerPort => 80;
    public int HealthCheckGracePeriodSeconds => 300;

    // ALB Configuration
    public int AlbHealthCheckIntervalSeconds => 30;
    public int AlbHealthyThresholdCount => 2;
    public int AlbUnhealthyThresholdCount => 5;
    public string AlbHealthCheckPath => "/";
}
