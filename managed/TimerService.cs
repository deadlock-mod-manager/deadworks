using DeadworksManaged.Api;
using ITimer = DeadworksManaged.Api.ITimer;

namespace DeadworksManaged;

/// <summary>
/// Per-plugin ITimer implementation. Tracks all timers created by a plugin
/// so they can be cancelled when the plugin unloads.
/// </summary>
internal sealed class TimerService : ITimer, IDisposable
{
    private readonly List<TimerHandle> _handles = new();
    private readonly object _lock = new();
    private bool _disposed;

    private TimerHandle Track(ScheduledTask task)
    {
        var handle = new TimerHandle(task);
        task.Handle = handle;

        lock (_lock)
        {
            if (_disposed)
            {
                task.Cancelled = true;
                task.Finished = true;
                handle.NotifyFinished();
                return handle;
            }
            _handles.Add(handle);
        }

        return handle;
    }

    public IHandle Once(Duration delay, Action callback)
    {
        var task = TimerEngine.Schedule(delay, callback, repeating: false);
        return Track(task);
    }

    public IHandle Every(Duration interval, Action callback)
    {
        var task = TimerEngine.Schedule(interval, callback, repeating: true);
        return Track(task);
    }

    public IHandle Sequence(Func<IStep, Pace> callback)
    {
        var task = TimerEngine.ScheduleSequence(callback);
        return Track(task);
    }

    public void NextTick(Action callback)
    {
        TimerEngine.EnqueueNextTick(callback);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;
            _disposed = true;

            foreach (var handle in _handles)
                handle.Cancel();
            _handles.Clear();
        }
    }
}
