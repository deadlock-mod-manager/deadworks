namespace DeadworksManaged.Api;

internal static class ConfigResolver
{
    internal static Func<IDeadworksPlugin, bool>? ReloadConfig;
    internal static Func<IDeadworksPlugin, string?>? GetConfigPath;
}
