namespace DeadworksManaged.Api;

/// <summary>Fired just before an entity is deleted. Passed to <see cref="IDeadworksPlugin.OnEntityDeleted"/>.</summary>
public sealed class EntityDeletedEvent {
	public required CBaseEntity Entity { get; init; }
}
