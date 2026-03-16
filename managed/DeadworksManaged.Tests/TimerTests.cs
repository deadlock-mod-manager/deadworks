using DeadworksManaged;
using DeadworksManaged.Api;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DeadworksManaged.Tests;

/// <summary>
/// Minimal IDeadworksPlugin stub for testing.
/// </summary>
internal sealed class StubPlugin : IDeadworksPlugin
{
    public string Name => "StubPlugin";
    public void OnLoad(bool isReload) { }
    public void OnUnload() { }
}

/// <summary>
/// Helper to drive TimerEngine ticks in tests.
/// </summary>
internal static class TestHelper
{
    /// <summary>Initialize the timer system for testing.</summary>
    public static (TimerService service, StubPlugin plugin) Setup()
    {
        TimerEngine.Reset();
        TimerRegistry.Initialize();

        var plugin = new StubPlugin();
        var service = new TimerService();
        TimerRegistry.Register(plugin, service);

        return (service, plugin);
    }

    /// <summary>Advance the timer engine by N ticks.</summary>
    public static void Tick(int count = 1)
    {
        for (var i = 0; i < count; i++)
            TimerEngine.OnTick();
    }

    public static void Teardown(StubPlugin plugin, TimerService service)
    {
        service.Dispose();
        TimerRegistry.Unregister(plugin);
        TimerEngine.Reset();
    }
}

public class TimerOnceTests
{
    [Fact]
    public void Once_fires_after_specified_ticks()
    {
        var (service, plugin) = TestHelper.Setup();
        var fired = false;

        service.Once(3.Ticks(), () => fired = true);

        TestHelper.Tick(2);
        Assert.False(fired);

        TestHelper.Tick(1);
        Assert.True(fired);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Once_fires_only_once()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        service.Once(1.Ticks(), () => count++);

        TestHelper.Tick(5);
        Assert.Equal(1, count);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Once_handle_IsFinished_after_firing()
    {
        var (service, plugin) = TestHelper.Setup();

        var handle = service.Once(1.Ticks(), () => { });
        Assert.False(handle.IsFinished);

        TestHelper.Tick(1);
        Assert.True(handle.IsFinished);

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerEveryTests
{
    [Fact]
    public void Every_fires_repeatedly()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        service.Every(2.Ticks(), () => count++);

        TestHelper.Tick(6);
        Assert.Equal(3, count);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Every_does_not_fire_before_interval()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        service.Every(5.Ticks(), () => count++);

        TestHelper.Tick(4);
        Assert.Equal(0, count);

        TestHelper.Tick(1);
        Assert.Equal(1, count);

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerCancelTests
{
    [Fact]
    public void Cancel_prevents_firing()
    {
        var (service, plugin) = TestHelper.Setup();
        var fired = false;

        var handle = service.Once(3.Ticks(), () => fired = true);
        TestHelper.Tick(1);
        handle.Cancel();

        TestHelper.Tick(5);
        Assert.False(fired);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Cancel_stops_repeating_timer()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        var handle = service.Every(1.Ticks(), () => count++);

        TestHelper.Tick(3);
        Assert.Equal(3, count);

        handle.Cancel();
        TestHelper.Tick(3);
        Assert.Equal(3, count); // no more increments

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Cancel_is_idempotent()
    {
        var (service, plugin) = TestHelper.Setup();

        var handle = service.Once(1.Ticks(), () => { });
        handle.Cancel();
        handle.Cancel(); // should not throw

        Assert.True(handle.IsFinished);

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerNextTickTests
{
    [Fact]
    public void NextTick_executes_on_next_tick()
    {
        var (service, plugin) = TestHelper.Setup();
        var fired = false;

        service.NextTick(() => fired = true);
        Assert.False(fired);

        TestHelper.Tick(1);
        Assert.True(fired);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void NextTick_only_fires_once()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        service.NextTick(() => count++);

        TestHelper.Tick(3);
        Assert.Equal(1, count);

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerSequenceTests
{
    [Fact]
    public void Sequence_runs_multiple_steps()
    {
        var (service, plugin) = TestHelper.Setup();
        var runs = 0;

        service.Sequence(step =>
        {
            runs++;
            return step.Run < 3 ? step.Wait(1.Ticks()) : step.Done();
        });

        TestHelper.Tick(5);
        Assert.Equal(3, runs);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Sequence_wait_delays_next_step()
    {
        var (service, plugin) = TestHelper.Setup();
        var runs = 0;

        service.Sequence(step =>
        {
            runs++;
            return step.Run < 2 ? step.Wait(3.Ticks()) : step.Done();
        });

        // First run at tick 1, wait 3 ticks, second run at tick 4
        TestHelper.Tick(1);
        Assert.Equal(1, runs);

        TestHelper.Tick(2);
        Assert.Equal(1, runs); // still waiting

        TestHelper.Tick(1);
        Assert.Equal(2, runs); // second run

        TestHelper.Tick(5);
        Assert.Equal(2, runs); // done, no more runs

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Sequence_handle_finishes_on_done()
    {
        var (service, plugin) = TestHelper.Setup();

        var handle = service.Sequence(step => step.Done());

        TestHelper.Tick(1);
        Assert.True(handle.IsFinished);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Sequence_ElapsedTicks_increments()
    {
        var (service, plugin) = TestHelper.Setup();
        long capturedElapsed = -1;

        service.Sequence(step =>
        {
            if (step.Run == 3)
            {
                capturedElapsed = step.ElapsedTicks;
                return step.Done();
            }
            return step.Wait(1.Ticks());
        });

        TestHelper.Tick(5);
        Assert.True(capturedElapsed >= 2, $"Expected ElapsedTicks >= 2, got {capturedElapsed}");

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerServiceDisposeTests
{
    [Fact]
    public void Dispose_cancels_all_owned_timers()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        service.Every(1.Ticks(), () => count++);
        service.Every(1.Ticks(), () => count++);

        TestHelper.Tick(2);
        Assert.True(count > 0);

        var snapshot = count;
        service.Dispose();

        TestHelper.Tick(5);
        Assert.Equal(snapshot, count); // no more increments

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Timers_created_after_dispose_are_immediately_cancelled()
    {
        var (service, plugin) = TestHelper.Setup();
        service.Dispose();

        var fired = false;
        var handle = service.Once(1.Ticks(), () => fired = true);

        TestHelper.Tick(5);
        Assert.False(fired);
        Assert.True(handle.IsFinished);

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerPluginInterfaceTests
{
    [Fact]
    public void Plugin_Timer_property_resolves_correctly()
    {
        var (service, plugin) = TestHelper.Setup();

        var timer = ((IDeadworksPlugin)plugin).Timer;
        Assert.NotNull(timer);
        Assert.Same(service, timer);

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Plugin_Timer_throws_when_not_registered()
    {
        TimerEngine.Reset();
        TimerRegistry.Initialize();

        var unregistered = new StubPlugin();
        Assert.Throws<InvalidOperationException>(() => ((IDeadworksPlugin)unregistered).Timer);

        TimerEngine.Reset();
    }
}

public class DurationTests
{
    [Fact]
    public void Ticks_extension_creates_tick_duration()
    {
        var d = 64.Ticks();
        Assert.Equal(64, d.Value);
        Assert.Equal(DurationKind.Ticks, d.Kind);
    }

    [Fact]
    public void Seconds_extension_creates_realtime_duration()
    {
        var d = 3.Seconds();
        Assert.Equal(3000, d.Value);
        Assert.Equal(DurationKind.RealTime, d.Kind);
    }

    [Fact]
    public void Milliseconds_extension_creates_realtime_duration()
    {
        var d = 500.Milliseconds();
        Assert.Equal(500, d.Value);
        Assert.Equal(DurationKind.RealTime, d.Kind);
    }

    [Fact]
    public void Double_seconds_extension_works()
    {
        var d = 1.5.Seconds();
        Assert.Equal(1500, d.Value);
        Assert.Equal(DurationKind.RealTime, d.Kind);
    }
}

public class TimerEngineResetTests
{
    [Fact]
    public void Reset_clears_all_state()
    {
        var (service, plugin) = TestHelper.Setup();
        var count = 0;

        service.Every(1.Ticks(), () => count++);
        TestHelper.Tick(3);
        Assert.True(count > 0);

        TimerEngine.Reset();
        var prevCount = count;

        TestHelper.Tick(5);
        Assert.Equal(prevCount, count); // timers cleared

        TestHelper.Teardown(plugin, service);
    }
}

public class TimerExceptionHandlingTests
{
    [Fact]
    public void Throwing_callback_does_not_break_engine()
    {
        var (service, plugin) = TestHelper.Setup();
        var secondFired = false;

        service.Once(1.Ticks(), () => throw new Exception("boom"));
        service.Once(1.Ticks(), () => secondFired = true);

        TestHelper.Tick(1);
        Assert.True(secondFired); // engine continued despite first timer throwing

        TestHelper.Teardown(plugin, service);
    }

    [Fact]
    public void Throwing_sequence_finishes_the_handle()
    {
        var (service, plugin) = TestHelper.Setup();

        var handle = service.Sequence(step => throw new Exception("boom"));

        TestHelper.Tick(1);
        Assert.True(handle.IsFinished);

        TestHelper.Teardown(plugin, service);
    }
}
