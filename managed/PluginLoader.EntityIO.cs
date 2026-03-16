using DeadworksManaged.Api;

namespace DeadworksManaged;

internal static partial class PluginLoader
{
    // --- Entity IO hooks ---

    private static IHandle OnEntityIOHookOutput(string designerName, string outputName, Action<EntityOutputEvent> handler)
    {
        var key = $"{designerName}:{outputName}";
        lock (_lock)
        {
            if (!_outputHooks.TryGetValue(key, out var list))
            {
                list = new List<Action<EntityOutputEvent>>();
                _outputHooks[key] = list;
            }
            list.Add(handler);
        }

        return new CallbackHandle(() =>
        {
            lock (_lock)
            {
                if (_outputHooks.TryGetValue(key, out var list))
                {
                    list.Remove(handler);
                    if (list.Count == 0)
                        _outputHooks.Remove(key);
                }
            }
        });
    }

    private static IHandle OnEntityIOHookInput(string designerName, string inputName, Action<EntityInputEvent> handler)
    {
        var key = $"{designerName}:{inputName}";
        lock (_lock)
        {
            if (!_inputHooks.TryGetValue(key, out var list))
            {
                list = new List<Action<EntityInputEvent>>();
                _inputHooks[key] = list;
            }
            list.Add(handler);
        }

        return new CallbackHandle(() =>
        {
            lock (_lock)
            {
                if (_inputHooks.TryGetValue(key, out var list))
                {
                    list.Remove(handler);
                    if (list.Count == 0)
                        _inputHooks.Remove(key);
                }
            }
        });
    }

    public static void DispatchEntityFireOutput(string designerName, EntityOutputEvent evt)
    {
        var key = $"{designerName}:{evt.OutputName}";
        List<Action<EntityOutputEvent>>? handlers;
        lock (_lock)
        {
            if (!_outputHooks.TryGetValue(key, out handlers))
                return;
            handlers = [.. handlers]; // snapshot
        }

        foreach (var handler in handlers)
        {
            try
            {
                handler(evt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PluginLoader] Entity output hook '{key}' threw: {ex.Message}");
            }
        }
    }

    public static void DispatchEntityAcceptInput(string designerName, EntityInputEvent evt)
    {
        var key = $"{designerName}:{evt.InputName}";
        List<Action<EntityInputEvent>>? handlers;
        lock (_lock)
        {
            if (!_inputHooks.TryGetValue(key, out handlers))
                return;
            handlers = [.. handlers]; // snapshot
        }

        foreach (var handler in handlers)
        {
            try
            {
                handler(evt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PluginLoader] Entity input hook '{key}' threw: {ex.Message}");
            }
        }
    }
}
