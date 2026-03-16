namespace DeadworksManaged.Api;

/// <summary>Fired when a new entity is created. Passed to <see cref="IDeadworksPlugin.OnEntityCreated"/>.</summary>
public sealed class EntityCreatedEvent {
	public required CBaseEntity Entity { get; init; }
}
