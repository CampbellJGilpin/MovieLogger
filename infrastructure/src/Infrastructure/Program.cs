using Amazon.CDK;
using Infrastructure.Config;
using Infrastructure.Stacks;
using Infrastructure.Utils;

namespace Infrastructure;

public class Program
{
    public static void Main(string[] args)
    {
        var app = new App();

        // Load configuration
        var config = EnvironmentConfig.Load();

        // Create CDK environment
        var cdkEnvironment = new Amazon.CDK.Environment
        {
            Account = config.AccountId,
            Region = config.Region
        };

        // Create stacks
        var baseStack = new BaseStack(app, Naming.CreateStackName("Base", config.Environment), config, new StackProps
        {
            Env = cdkEnvironment
        });

        var storageStack = new StorageStack(app, Naming.CreateStackName("Storage", config.Environment), config, new StackProps
        {
            Env = cdkEnvironment
        });

        var databaseStack = new DatabaseStack(app, Naming.CreateStackName("Database", config.Environment), config, baseStack.Vpc, baseStack.DatabaseSecurityGroup, new StackProps
        {
            Env = cdkEnvironment
        });

        var cdnStack = new CdnStack(app, Naming.CreateStackName("CDN", config.Environment), config, storageStack.MovieBucket, new StackProps
        {
            Env = cdkEnvironment
        });

        var computeStack = new ComputeStack(app, Naming.CreateStackName("Compute", config.Environment), config, new StackProps
        {
            Env = cdkEnvironment
        });

        // Add dependencies
        storageStack.AddDependency(baseStack);
        databaseStack.AddDependency(baseStack);
        cdnStack.AddDependency(storageStack);
        computeStack.AddDependency(baseStack);

        app.Synth();
    }
}
