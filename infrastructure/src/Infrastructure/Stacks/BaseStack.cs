using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Constructs;
using Infrastructure.Config;
using Infrastructure.Utils;

namespace Infrastructure.Stacks;

public class BaseStack : Stack
{
    public Vpc Vpc { get; }
    public SecurityGroup DatabaseSecurityGroup { get; }
    public SecurityGroup LightsailSecurityGroup { get; }

    public BaseStack(Construct scope, string id, EnvironmentConfig config, IStackProps? props = null) 
        : base(scope, id, props)
    {
        // Create VPC
        Vpc = new Vpc(this, "MovieLoggerVpc", new VpcProps
        {
            MaxAzs = 2,
            NatGateways = 1,
            SubnetConfiguration = new[]
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
                    Name = "Database",
                    SubnetType = SubnetType.PRIVATE_ISOLATED,
                    CidrMask = 24
                }
            }
        });

        // Database Security Group
        DatabaseSecurityGroup = new SecurityGroup(this, "DatabaseSecurityGroup", new SecurityGroupProps
        {
            Vpc = Vpc,
            Description = "Security group for RDS database",
            AllowAllOutbound = false,
            SecurityGroupName = Naming.CreateSecurityGroupName("database", config.Environment)
        });

        // Lightsail Security Group
        LightsailSecurityGroup = new SecurityGroup(this, "LightsailSecurityGroup", new SecurityGroupProps
        {
            Vpc = Vpc,
            Description = "Security group for Lightsail instance",
            AllowAllOutbound = true,
            SecurityGroupName = Naming.CreateSecurityGroupName("lightsail", config.Environment)
        });

        // Allow Lightsail to access database
        DatabaseSecurityGroup.AddIngressRule(
            LightsailSecurityGroup,
            Port.Tcp(5432),
            "Allow Lightsail to access PostgreSQL"
        );

        // Allow SSH access to Lightsail
        LightsailSecurityGroup.AddIngressRule(
            Peer.AnyIpv4(),
            Port.Tcp(22),
            "Allow SSH access"
        );

        // Allow HTTP/HTTPS access to Lightsail
        LightsailSecurityGroup.AddIngressRule(
            Peer.AnyIpv4(),
            Port.Tcp(80),
            "Allow HTTP access"
        );

        LightsailSecurityGroup.AddIngressRule(
            Peer.AnyIpv4(),
            Port.Tcp(443),
            "Allow HTTPS access"
        );

        // Add tags
        TagUtils.AddCommonTags(Vpc, config.Environment);
        TagUtils.AddPurposeTag(Vpc, "Networking");
        
        TagUtils.AddCommonTags(DatabaseSecurityGroup, config.Environment);
        TagUtils.AddPurposeTag(DatabaseSecurityGroup, "Database");
        
        TagUtils.AddCommonTags(LightsailSecurityGroup, config.Environment);
        TagUtils.AddPurposeTag(LightsailSecurityGroup, "Compute");
    }
} 