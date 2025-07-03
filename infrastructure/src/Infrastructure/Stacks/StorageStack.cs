using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Constructs;
using Infrastructure.Config;
using Infrastructure.Utils;

namespace Infrastructure.Stacks;

public class StorageStack : Stack
{
    public Bucket MovieBucket { get; }

    public StorageStack(Construct scope, string id, EnvironmentConfig config, IStackProps? props = null) 
        : base(scope, id, props)
    {
        MovieBucket = new Bucket(this, "MovieLoggerBucket", new BucketProps
        {
            BucketName = config.BucketName,
            Versioned = config.EnableVersioning,
            BlockPublicAccess = BlockPublicAccess.BLOCK_ACLS_ONLY,
            RemovalPolicy = config.Environment == "prod" ? RemovalPolicy.RETAIN : RemovalPolicy.DESTROY,
            AutoDeleteObjects = config.Environment != "prod",
            Cors = new[]
            {
                new CorsRule
                {
                    AllowedMethods = new[] { HttpMethods.GET, HttpMethods.PUT, HttpMethods.POST, HttpMethods.DELETE },
                    AllowedOrigins = new[] { "*" },
                    AllowedHeaders = new[] { "*" },
                    ExposedHeaders = new[] { "ETag" },
                    MaxAge = 3000
                }
            },
            LifecycleRules = new[]
            {
                new LifecycleRule
                {
                    Id = "DeleteOldVersions",
                    NoncurrentVersionExpiration = Duration.Days(30),
                    Enabled = true
                },
                new LifecycleRule
                {
                    Id = "DeleteIncompleteMultipartUploads",
                    AbortIncompleteMultipartUploadAfter = Duration.Days(7),
                    Enabled = true
                }
            },
            Encryption = BucketEncryption.S3_MANAGED,
            EnforceSSL = true,
            ServerAccessLogsPrefix = "access-logs"
        });

        // Add tags
        TagUtils.AddCommonTags(MovieBucket, config.Environment);
        TagUtils.AddPurposeTag(MovieBucket, "MovieStorage");
        TagUtils.AddCostCenterTag(MovieBucket);
    }
} 