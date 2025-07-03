namespace Infrastructure.Utils;

public static class Naming
{
    public static string CreateResourceName(string resourceType, string environment, string? suffix = null)
    {
        var name = $"movielogger-{resourceType}-{environment}";
        return suffix != null ? $"{name}-{suffix}" : name;
    }

    public static string CreateStackName(string stackType, string environment)
    {
        return $"MovieLogger-{stackType}-{environment}";
    }

    public static string CreateSecurityGroupName(string purpose, string environment)
    {
        return $"movielogger-{purpose}-sg-{environment}";
    }

    public static string CreateSubnetGroupName(string environment)
    {
        return $"movielogger-db-subnet-{environment}";
    }

    public static string CreateParameterGroupName(string environment)
    {
        return $"movielogger-db-params-{environment}";
    }
} 