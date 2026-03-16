namespace DeadworksManaged.Api;

/// <summary>Body component attached to an entity, providing access to the <see cref="CGameSceneNode"/> and world position.</summary>
public unsafe class CBodyComponent : NativeEntity {
	internal CBodyComponent(nint handle) : base(handle) { }

	private static readonly SchemaAccessor<nint> _sceneNode = new("CBodyComponent"u8, "m_pSceneNode"u8);
	public CGameSceneNode? SceneNode {
		get {
			nint ptr = _sceneNode.Get(Handle);
			return ptr != 0 ? new CGameSceneNode(ptr) : null;
		}
	}
}
