namespace DeadworksManaged.Api;

/// <summary>
/// Internal resolver used by the IDeadworksPlugin.Timer default property.
/// Set up by the host during initialization.
/// </summary>
internal static class TimerResolver
{
    internal static Func<IDeadworksPlugin, ITimer>? Resolve;

    public static ITimer Get(IDeadworksPlugin plugin)
    {
        if (Resolve == null)
            throw new InvalidOperationException("Timer system not initialized.");
        return Resolve(plugin);
    }
}
