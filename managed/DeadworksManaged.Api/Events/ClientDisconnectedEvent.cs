namespace DeadworksManaged.Api;

/// <summary>Fired when a client disconnects. Passed to <see cref="IDeadworksPlugin.OnClientDisconnect"/>.</summary>
public sealed class ClientDisconnectedEvent {
	public required int Slot { get; init; }
	public required int Reason { get; init; }

	[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
	public unsafe CCitadelPlayerController? Controller {
		get {
			if (NativeInterop.GetPlayerController == null)
				return null;
			var ptr = NativeInterop.GetPlayerController(Slot);
			return ptr != null ? new CCitadelPlayerController((nint)ptr) : null;
		}
	}
}
