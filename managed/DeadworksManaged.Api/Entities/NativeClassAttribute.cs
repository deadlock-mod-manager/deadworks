namespace DeadworksManaged.Api;

/// <summary>Maps C# entity wrapper types to their accepted C++ DLL class names.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class NativeClassAttribute : Attribute {
	public string[] ClassNames { get; }
	public NativeClassAttribute(params string[] classNames) => ClassNames = classNames;
}
