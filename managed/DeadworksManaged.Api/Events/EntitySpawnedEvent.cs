namespace DeadworksManaged.Api;

/// <summary>Fired when an entity has been fully spawned. Passed to <see cref="IDeadworksPlugin.OnEntitySpawned"/>.</summary>
public sealed class EntitySpawnedEvent {
	public required CBaseEntity Entity { get; init; }
}
