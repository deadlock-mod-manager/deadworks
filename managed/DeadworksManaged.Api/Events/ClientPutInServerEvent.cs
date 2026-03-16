namespace DeadworksManaged.Api;

/// <summary>Fired when a client is put into the server (initial connection). Passed to <see cref="IDeadworksPlugin.OnClientPutInServer"/>.</summary>
public sealed class ClientPutInServerEvent {
	public required int Slot { get; init; }
	public required string Name { get; init; }
	public required ulong Xuid { get; init; }
	public required bool IsBot { get; init; }

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
