namespace DeadworksManaged.Api;

/// <summary>Event data passed to <see cref="IDeadworksPlugin.OnTakeDamage"/>. Contains the target entity and full damage info.</summary>
public sealed class TakeDamageEvent {
	public required CBaseEntity Entity { get; init; }
	public required CTakeDamageInfo Info { get; init; }
}
