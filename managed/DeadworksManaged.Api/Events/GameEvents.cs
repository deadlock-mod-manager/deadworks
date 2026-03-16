namespace DeadworksManaged.Api;

/// <summary>Static API for registering/removing dynamic Source 2 game event listeners and creating new events.</summary>
public static class GameEvents {
	internal static Func<string, GameEventHandler, IHandle>? OnAddListener;
	internal static Action<string, GameEventHandler>? OnRemoveListener;

	/// <summary>Registers a dynamic listener for the named game event. Returns a handle that cancels the listener when disposed or cancelled.</summary>
	public static IHandle AddListener(string eventName, GameEventHandler handler) {
		return OnAddListener?.Invoke(eventName, handler) ?? CallbackHandle.Noop;
	}

	/// <summary>Removes a previously registered dynamic game event listener.</summary>
	public static void RemoveListener(string eventName, GameEventHandler handler) {
		OnRemoveListener?.Invoke(eventName, handler);
	}

	/// <summary>Creates a game event. Returns null if the event type doesn't exist. Must be fired or freed.</summary>
	public static unsafe GameEventWriter? Create(string name, bool force = true) {
		Span<byte> utf8 = Utf8.Encode(name, stackalloc byte[Utf8.Size(name)]);
		fixed (byte* namePtr = utf8)
		{
			var ptr = NativeInterop.CreateGameEvent(namePtr, force ? (byte)1 : (byte)0);
			return ptr != null ? new GameEventWriter(ptr) : null;
		}
	}
}
