namespace DeadworksManaged.Api;

/// <summary>Event data passed to <see cref="IDeadworksPlugin.OnClientConCommand"/>. Contains the controller pointer, command name, and parsed argument array.</summary>
public sealed class ClientConCommandEvent {
	public required nint ControllerPtr { get; init; }
	public required string Command { get; init; }
	public required string[] Args { get; init; }

	[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
	public CCitadelPlayerController? Controller {
		get {
			return ControllerPtr != 0 ? new CCitadelPlayerController(ControllerPtr) : null;
		}
	}
}
