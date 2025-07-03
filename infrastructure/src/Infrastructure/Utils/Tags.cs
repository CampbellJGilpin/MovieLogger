using Amazon.CDK;
using Constructs;

namespace Infrastructure.Utils;

public static class TagUtils
{
    public static void AddCommonTags(IConstruct resource, string environment, string project = "MovieLogger")
    {
        Amazon.CDK.Tags.Of(resource).Add("Environment", environment);
        Amazon.CDK.Tags.Of(resource).Add("Project", project);
        Amazon.CDK.Tags.Of(resource).Add("ManagedBy", "CDK");
    }

    public static void AddPurposeTag(IConstruct resource, string purpose)
    {
        Amazon.CDK.Tags.Of(resource).Add("Purpose", purpose);
    }

    public static void AddCostCenterTag(IConstruct resource, string costCenter = "MovieLogger")
    {
        Amazon.CDK.Tags.Of(resource).Add("CostCenter", costCenter);
    }
} 