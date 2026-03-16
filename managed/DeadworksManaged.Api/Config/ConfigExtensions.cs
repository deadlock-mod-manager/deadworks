namespace DeadworksManaged.Api;

public static class ConfigExtensions
{
    public static bool ReloadConfig(this IDeadworksPlugin plugin)
    {
        if (ConfigResolver.ReloadConfig == null)
            throw new InvalidOperationException("Config system not initialized.");
        return ConfigResolver.ReloadConfig(plugin);
    }

    public static string? GetConfigPath(this IDeadworksPlugin plugin)
    {
        if (ConfigResolver.GetConfigPath == null)
            throw new InvalidOperationException("Config system not initialized.");
        return ConfigResolver.GetConfigPath(plugin);
    }
}
