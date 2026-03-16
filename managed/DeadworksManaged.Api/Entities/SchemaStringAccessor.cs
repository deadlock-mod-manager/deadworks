namespace DeadworksManaged.Api;

/// <summary>Write-only schema accessor for CUtlSymbolLarge (string) fields. Calls native SetSchemaString.</summary>
public sealed unsafe class SchemaStringAccessor {
	private readonly byte[] _className;
	private readonly byte[] _fieldName;

	public SchemaStringAccessor(ReadOnlySpan<byte> className, ReadOnlySpan<byte> fieldName) {
		_className = new byte[className.Length + 1];
		className.CopyTo(_className);
		_fieldName = new byte[fieldName.Length + 1];
		fieldName.CopyTo(_fieldName);
	}

	/// <summary>Sets the CUtlSymbolLarge string field on <paramref name="entity"/> to <paramref name="value"/>.</summary>
	public void Set(nint entity, string value) {
		Span<byte> utf8Val = Utf8.Encode(value, stackalloc byte[Utf8.Size(value)]);
		fixed (byte* cls = _className, fld = _fieldName, val = utf8Val) {
			NativeInterop.SetSchemaString((void*)entity, cls, fld, val);
		}
	}
}
