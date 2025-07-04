using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.S3;
using Constructs;
using Infrastructure.Config;
using Infrastructure.Utils;

namespace Infrastructure.Stacks;

public class CdnStack : Stack
{
    public Distribution CloudFrontDistribution { get; }

    public CdnStack(Construct scope, string id, EnvironmentConfig config, Bucket s3Bucket, IStackProps? props = null) 
        : base(scope, id, props)
    {
        // Create OAI for S3 access
        var originAccessIdentity = new OriginAccessIdentity(this, "CloudFrontOAI", new OriginAccessIdentityProps
        {
            Comment = $"OAI for MovieLogger {config.Environment} environment"
        });

        // Create S3 origin without automatic permissions
        var origin = new S3Origin(s3Bucket, new S3OriginProps
        {
            OriginAccessIdentity = originAccessIdentity
        });

        // Create CloudFront distribution
        CloudFrontDistribution = new Distribution(this, Naming.CreateResourceName("Distribution", config.Environment), new DistributionProps
        {
            DefaultBehavior = new BehaviorOptions
            {
                Origin = origin,
                ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
                AllowedMethods = AllowedMethods.ALLOW_ALL,
                CachedMethods = CachedMethods.CACHE_GET_HEAD,
                Compress = true,
                CachePolicy = CachePolicy.CACHING_OPTIMIZED
            },
            PriceClass = PriceClass.PRICE_CLASS_100,
            Enabled = true,
            HttpVersion = HttpVersion.HTTP2,
            DefaultRootObject = "index.html",
            ErrorResponses = new[]
            {
                new ErrorResponse
                {
                    HttpStatus = 403,
                    ResponseHttpStatus = 200,
                    ResponsePagePath = "/index.html"
                },
                new ErrorResponse
                {
                    HttpStatus = 404,
                    ResponseHttpStatus = 200,
                    ResponsePagePath = "/index.html"
                }
            }
        });

        // Add tags
        TagUtils.AddCommonTags(CloudFrontDistribution, config.Environment);
        TagUtils.AddPurposeTag(CloudFrontDistribution, "CDN");
        TagUtils.AddCostCenterTag(CloudFrontDistribution);
    }
} 