namespace DeadworksManaged.Api;

/// <summary>Data for a received entity input, passed to handlers registered via <see cref="EntityIO.HookInput"/>.</summary>
public sealed class EntityInputEvent {
	/// <summary>The entity receiving the input.</summary>
	public required CBaseEntity Entity { get; init; }

	/// <summary>The entity that activated the input, if any.</summary>
	public CBaseEntity? Activator { get; init; }

	/// <summary>The entity that called the input, if any.</summary>
	public CBaseEntity? Caller { get; init; }

	/// <summary>The name of the input being received (e.g. <c>"Kill"</c>).</summary>
	public required string InputName { get; init; }

	/// <summary>Optional string value passed with the input, or <see langword="null"/> if none.</summary>
	public string? Value { get; init; }
}
