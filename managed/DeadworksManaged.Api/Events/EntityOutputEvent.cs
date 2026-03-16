namespace DeadworksManaged.Api;

/// <summary>Data for a fired entity output, passed to handlers registered via <see cref="EntityIO.HookOutput"/>.</summary>
public sealed class EntityOutputEvent {
	/// <summary>The entity that fired the output.</summary>
	public required CBaseEntity Entity { get; init; }

	/// <summary>The entity that activated the output, if any.</summary>
	public CBaseEntity? Activator { get; init; }

	/// <summary>The entity that called the output, if any.</summary>
	public CBaseEntity? Caller { get; init; }

	/// <summary>The name of the output that fired (e.g. <c>"OnTrigger"</c>).</summary>
	public required string OutputName { get; init; }
}
