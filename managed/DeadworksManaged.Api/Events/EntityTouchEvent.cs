namespace DeadworksManaged.Api;

/// <summary>Fired when two entities start or stop touching. Passed to <see cref="IDeadworksPlugin.OnEntityStartTouch"/> and <see cref="IDeadworksPlugin.OnEntityEndTouch"/>.</summary>
public sealed class EntityTouchEvent {
	public required CBaseEntity Entity { get; init; }
	public required CBaseEntity Other { get; init; }
}
