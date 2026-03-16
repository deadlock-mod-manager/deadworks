using System.Numerics;

namespace DeadworksManaged.Api;

/// <summary>Scene graph node that holds the entity's world transform and absolute origin.</summary>
public unsafe class CGameSceneNode : NativeEntity {
	internal CGameSceneNode(nint handle) : base(handle) { }

	private static readonly SchemaAccessor<Vector3> _vecAbsOrigin = new("CGameSceneNode"u8, "m_vecAbsOrigin"u8);
	public Vector3 AbsOrigin => _vecAbsOrigin.Get(Handle);
}
