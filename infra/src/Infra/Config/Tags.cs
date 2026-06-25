using Amazon.CDK;
using Constructs;
using Infra.Config;

namespace Infra.Config;

public static class Tags
{
    public static void ApplyStandardTags(Construct scope, EnvironmentConfig config)
    {
        Amazon.CDK.Tags.Of(scope).Add("purpose", "modern");
        Amazon.CDK.Tags.Of(scope).Add("owner", "atx");
        Amazon.CDK.Tags.Of(scope).Add("project", config.ProjectName);
        Amazon.CDK.Tags.Of(scope).Add("environment", config.EnvironmentName);
        Amazon.CDK.Tags.Of(scope).Add("managed-by", "cdk");
        Amazon.CDK.Tags.Of(scope).Add("created-by", "aws-transform");
    }
}
