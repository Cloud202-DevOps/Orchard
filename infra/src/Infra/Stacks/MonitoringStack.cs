using Amazon.CDK;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.CloudWatch.Actions;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Constructs;
using Infra.Config;

namespace Infra.Stacks;

public class MonitoringStackProps : StackProps
{
    public required ICluster EcsCluster { get; set; }
    public required IBaseService EcsService { get; set; }
    public required IApplicationLoadBalancer LoadBalancer { get; set; }
}

public class MonitoringStack : Stack
{
    public MonitoringStack(Construct scope, string id, MonitoringStackProps props) : base(scope, id, props)
    {
        var config = new EnvironmentConfig
        {
            AccountId = props.Env?.Account ?? "",
            Region = props.Env?.Region ?? "",
            EnvironmentName = "dev",
            ProjectName = "orchard-modernization-poc"
        };

        // SNS Topic for alarms
        var alarmTopic = new Topic(this, "AlarmTopic", new TopicProps
        {
            TopicName = $"{config.ProjectName}-alarms-{config.EnvironmentName}",
            DisplayName = "Orchard CMS POC Alarms"
        });

        // ECS CPU Utilization Alarm
        var cpuAlarm = new Alarm(this, "CpuAlarm", new AlarmProps
        {
            AlarmName = $"{config.ProjectName}-cpu-high-{config.EnvironmentName}",
            AlarmDescription = "ECS service CPU utilization exceeds 80% for 5 minutes",
            Metric = new Metric(new MetricProps
            {
                Namespace = "AWS/ECS",
                MetricName = "CPUUtilization",
                DimensionsMap = new Dictionary<string, string>
                {
                    ["ClusterName"] = props.EcsCluster.ClusterName,
                    ["ServiceName"] = props.EcsService.ServiceName
                },
                Statistic = "Average",
                Period = Duration.Minutes(1)
            }),
            Threshold = 80,
            EvaluationPeriods = 5,
            ComparisonOperator = ComparisonOperator.GREATER_THAN_THRESHOLD,
            TreatMissingData = TreatMissingData.MISSING
        });
        cpuAlarm.AddAlarmAction(new SnsAction(alarmTopic));

        // ECS Memory Utilization Alarm
        var memoryAlarm = new Alarm(this, "MemoryAlarm", new AlarmProps
        {
            AlarmName = $"{config.ProjectName}-memory-high-{config.EnvironmentName}",
            AlarmDescription = "ECS service memory utilization exceeds 85% for 5 minutes",
            Metric = new Metric(new MetricProps
            {
                Namespace = "AWS/ECS",
                MetricName = "MemoryUtilization",
                DimensionsMap = new Dictionary<string, string>
                {
                    ["ClusterName"] = props.EcsCluster.ClusterName,
                    ["ServiceName"] = props.EcsService.ServiceName
                },
                Statistic = "Average",
                Period = Duration.Minutes(1)
            }),
            Threshold = 85,
            EvaluationPeriods = 5,
            ComparisonOperator = ComparisonOperator.GREATER_THAN_THRESHOLD,
            TreatMissingData = TreatMissingData.MISSING
        });
        memoryAlarm.AddAlarmAction(new SnsAction(alarmTopic));

        // ALB 5xx Error Alarm
        var http5xxAlarm = new Alarm(this, "Http5xxAlarm", new AlarmProps
        {
            AlarmName = $"{config.ProjectName}-5xx-high-{config.EnvironmentName}",
            AlarmDescription = "ALB 5xx errors exceed 10 in 5 minutes",
            Metric = new Metric(new MetricProps
            {
                Namespace = "AWS/ApplicationELB",
                MetricName = "HTTPCode_Target_5XX_Count",
                DimensionsMap = new Dictionary<string, string>
                {
                    ["LoadBalancer"] = props.LoadBalancer.LoadBalancerFullName
                },
                Statistic = "Sum",
                Period = Duration.Minutes(5)
            }),
            Threshold = 10,
            EvaluationPeriods = 1,
            ComparisonOperator = ComparisonOperator.GREATER_THAN_THRESHOLD,
            TreatMissingData = TreatMissingData.NOT_BREACHING
        });
        http5xxAlarm.AddAlarmAction(new SnsAction(alarmTopic));

        // ALB Unhealthy Host Count Alarm
        var unhealthyHostAlarm = new Alarm(this, "UnhealthyHostAlarm", new AlarmProps
        {
            AlarmName = $"{config.ProjectName}-unhealthy-hosts-{config.EnvironmentName}",
            AlarmDescription = "ALB has unhealthy targets for 3 consecutive checks",
            Metric = new Metric(new MetricProps
            {
                Namespace = "AWS/ApplicationELB",
                MetricName = "UnHealthyHostCount",
                DimensionsMap = new Dictionary<string, string>
                {
                    ["LoadBalancer"] = props.LoadBalancer.LoadBalancerFullName
                },
                Statistic = "Maximum",
                Period = Duration.Minutes(1)
            }),
            Threshold = 1,
            EvaluationPeriods = 3,
            ComparisonOperator = ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
            TreatMissingData = TreatMissingData.NOT_BREACHING
        });
        unhealthyHostAlarm.AddAlarmAction(new SnsAction(alarmTopic));

        // Outputs
        _ = new CfnOutput(this, "AlarmTopicArn", new CfnOutputProps
        {
            Value = alarmTopic.TopicArn,
            ExportName = $"{config.ProjectName}-alarm-topic-arn"
        });
    }
}
