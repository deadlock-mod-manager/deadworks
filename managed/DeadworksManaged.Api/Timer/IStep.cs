namespace DeadworksManaged.Api;

/// <summary>
/// Sequence step context passed to <see cref="ITimer.Sequence"/> callbacks.
/// Provides execution count and methods to control pacing.
/// </summary>
public interface IStep
{
    /// <summary>How many times the sequence callback has been invoked (starts at 1).</summary>
    int Run { get; }

    /// <summary>Game ticks elapsed since the sequence started.</summary>
    long ElapsedTicks { get; }

    /// <summary>Execute again after the specified duration.</summary>
    Pace Wait(Duration delay);

    /// <summary>End the sequence.</summary>
    Pace Done();
}
