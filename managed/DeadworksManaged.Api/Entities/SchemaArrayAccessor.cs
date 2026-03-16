namespace DeadworksManaged.Api;

/// <summary>Schema accessor for array-typed fields. Reads/writes at offset + index * sizeof(T).</summary>
public sealed unsafe class SchemaArrayAccessor<T> where T : unmanaged {
	private volatile int _offset = -1;
	private short _chainOffset;
	private bool _networked;
	private readonly byte[] _className;
	private readonly byte[] _fieldName;
	private readonly int _networkStateChangedOffset;

	public SchemaArrayAccessor(ReadOnlySpan<byte> className, ReadOnlySpan<byte> fieldName, int networkStateChangedOffset = 0) {
		_className = new byte[className.Length + 1];
		className.CopyTo(_className);
		_fieldName = new byte[fieldName.Length + 1];
		fieldName.CopyTo(_fieldName);
		_networkStateChangedOffset = networkStateChangedOffset;
	}

	private void Resolve() {
		fixed (byte* cls = _className, fld = _fieldName) {
			SchemaFieldResult r;
			NativeInterop.GetSchemaField(cls, fld, &r);
			_chainOffset = r.ChainOffset;
			_networked = r.Networked != 0;
			_offset = r.Offset;
		}
	}

	/// <summary>Reads element <paramref name="index"/> from the array field on <paramref name="entity"/>.</summary>
	public T Get(nint entity, int index) {
		if (_offset < 0) Resolve();
		return *(T*)((byte*)entity + _offset + index * sizeof(T));
	}

	/// <summary>Writes element <paramref name="index"/> in the array field on <paramref name="entity"/>, notifying network state if needed.</summary>
	public void Set(nint entity, int index, T value) {
		if (_offset < 0) Resolve();
		int elementOffset = _offset + index * sizeof(T);
		*(T*)((byte*)entity + elementOffset) = value;
		if (_networked)
			NativeInterop.NotifyStateChanged((void*)entity, elementOffset, _chainOffset, _networkStateChangedOffset);
	}
}
