namespace DeadworksManaged.Api;

/// <summary>Wraps a native CBaseModifier instance — a buff/debuff applied to an entity via <see cref="CBaseEntity.AddModifier"/>.</summary>
public unsafe class CBaseModifier : NativeEntity {
	internal CBaseModifier(nint handle) : base(handle) { }
}
