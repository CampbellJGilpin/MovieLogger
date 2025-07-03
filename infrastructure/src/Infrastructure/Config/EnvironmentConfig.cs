namespace Infrastructure.Config;

public class EnvironmentConfig
{
    public string Environment { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string AccountId { get; set; } = string.Empty;
    public string AvailabilityZone { get; set; } = string.Empty;
    
    // S3 Configuration
    public string BucketName { get; set; } = string.Empty;
    public bool EnableVersioning { get; set; } = true;
    public bool BlockPublicAccess { get; set; } = true;
    
    // RDS Configuration
    public string DatabaseName { get; set; } = "movielogger";
    public string DatabaseUsername { get; set; } = "admin";
    public string DatabaseInstanceClass { get; set; } = "db.t3.micro";
    public string DatabaseEngine { get; set; } = "postgresql";
    
    // CloudFront Configuration
    public string DomainName { get; set; } = string.Empty;
    public string CertificateArn { get; set; } = string.Empty;
    
    // Lightsail Configuration
    public string InstanceName { get; set; } = string.Empty;
    public string InstanceType { get; set; } = "nano_2_0";
    public string BlueprintId { get; set; } = "ubuntu_20_04";

    public static EnvironmentConfig Load()
    {
        // Get environment from context or default to dev
        var environment = System.Environment.GetEnvironmentVariable("CDK_ENVIRONMENT") ?? "dev";
        
        return environment switch
        {
            "dev" => CreateDevConfig(),
            "staging" => CreateStagingConfig(),
            "prod" => CreateProdConfig(),
            _ => CreateDevConfig()
        };
    }

    public static EnvironmentConfig CreateDevConfig()
    {
        return new EnvironmentConfig
        {
            Environment = "dev",
            Region = "us-east-1",
            AvailabilityZone = "us-east-1a",
            AccountId = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT") ?? "",
            BucketName = "movielogger-dev-storage",
            DomainName = "dev.movielogger.com",
            InstanceName = "movielogger-dev"
        };
    }

    public static EnvironmentConfig CreateStagingConfig()
    {
        return new EnvironmentConfig
        {
            Environment = "staging",
            Region = "us-east-1",
            AvailabilityZone = "us-east-1a",
            AccountId = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT") ?? "",
            BucketName = "movielogger-staging-storage",
            DomainName = "staging.movielogger.com",
            InstanceName = "movielogger-staging"
        };
    }

    public static EnvironmentConfig CreateProdConfig()
    {
        return new EnvironmentConfig
        {
            Environment = "prod",
            Region = "us-east-1",
            AvailabilityZone = "us-east-1a",
            AccountId = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT") ?? "",
            BucketName = "movielogger-prod-storage",
            DomainName = "movielogger.com",
            InstanceName = "movielogger-prod",
            DatabaseInstanceClass = "db.t3.small"
        };
    }
} 