using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Constructs;
using Infrastructure.Config;
using Infrastructure.Utils;

namespace Infrastructure.Stacks;

public class DatabaseStack : Stack
{
    public DatabaseInstance Database { get; }

    public DatabaseStack(Construct scope, string id, EnvironmentConfig config, Vpc vpc, SecurityGroup securityGroup, IStackProps? props = null) 
        : base(scope, id, props)
    {
        // Create subnet group
        var subnetGroup = new CfnDBSubnetGroup(this, "DatabaseSubnetGroup", new CfnDBSubnetGroupProps
        {
            DbSubnetGroupDescription = "Subnet group for MovieLogger database",
            SubnetIds = vpc.PrivateSubnets.Select(s => s.SubnetId).ToArray(),
            DbSubnetGroupName = Naming.CreateSubnetGroupName(config.Environment)
        });

        // Create parameter group
        var parameterGroup = new CfnDBParameterGroup(this, "DatabaseParameterGroup", new CfnDBParameterGroupProps
        {
            Description = "Parameter group for MovieLogger database",
            Family = "postgres13",
            Parameters = new Dictionary<string, object>
            {
                { "timezone", "UTC" },
                { "log_statement", "all" },
                { "log_min_duration_statement", "1000" },
                { "shared_preload_libraries", "pg_stat_statements" }
            }
        });

        Database = new DatabaseInstance(this, Naming.CreateResourceName("Database", config.Environment), new DatabaseInstanceProps
        {
            Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
            {
                Version = PostgresEngineVersion.VER_16_2
            }),
            InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(Amazon.CDK.AWS.EC2.InstanceClass.T3, Amazon.CDK.AWS.EC2.InstanceSize.MICRO),
            DatabaseName = config.DatabaseName,
            Credentials = Credentials.FromGeneratedSecret(config.DatabaseUsername),
            Vpc = vpc,
            VpcSubnets = new SubnetSelection
            {
                SubnetType = SubnetType.PRIVATE_WITH_EGRESS
            },
            SecurityGroups = new[] { securityGroup },
            BackupRetention = Duration.Days(7),
            DeletionProtection = false,
            RemovalPolicy = config.Environment == "prod" ? RemovalPolicy.RETAIN : RemovalPolicy.DESTROY
        });

        // Add tags
        TagUtils.AddCommonTags(Database, config.Environment);
        TagUtils.AddPurposeTag(Database, "Database");
        TagUtils.AddCostCenterTag(Database);
    }
} 