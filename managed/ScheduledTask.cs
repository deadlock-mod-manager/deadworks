using DeadworksManaged.Api;

namespace DeadworksManaged;

/// <summary>
/// Internal scheduled task representation.
/// </summary>
internal sealed class ScheduledTask
{
    public Action? Callback;
    public Func<IStep, Pace>? SequenceCallback;
    public Duration Interval;
    public bool Repeating;
    public volatile bool Cancelled;
    public volatile bool Finished;
    public TimerHandle? Handle;

    // Sequence state
    public int RunCount;
    public long StartTick;
}
