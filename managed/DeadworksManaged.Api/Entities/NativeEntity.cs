namespace DeadworksManaged.Api;

/// <summary>Base class for all managed wrappers around native C++ entity/object pointers.</summary>
public abstract class NativeEntity {
	/// <summary>Raw pointer to the native object.</summary>
	public nint Handle { get; }
	/// <summary>True if the pointer is non-null.</summary>
	public bool IsValid => Handle != 0;

	protected NativeEntity(nint handle) => Handle = handle;
}
