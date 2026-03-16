namespace DeadworksManaged.Api;

/// <summary>Wraps a newly created IGameEvent for setting fields and firing.</summary>
public sealed unsafe class GameEventWriter : IDisposable {
	private void* _event;
	private bool _fired;

	internal GameEventWriter(void* eventPtr) { _event = eventPtr; }

	public GameEventWriter SetString(string key, string value) {
		if (_event == null) return this;
		Span<byte> keyUtf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		Span<byte> valUtf8 = Utf8.Encode(value, stackalloc byte[Utf8.Size(value)]);
		fixed (byte* k = keyUtf8, v = valUtf8)
			NativeInterop.GameEventSetString(_event, k, v);
		return this;
	}

	public GameEventWriter SetInt(string key, int value) {
		if (_event == null) return this;
		Span<byte> keyUtf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* k = keyUtf8)
			NativeInterop.GameEventSetInt(_event, k, value);
		return this;
	}

	public GameEventWriter SetFloat(string key, float value) {
		if (_event == null) return this;
		Span<byte> keyUtf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* k = keyUtf8)
			NativeInterop.GameEventSetFloat(_event, k, value);
		return this;
	}

	public GameEventWriter SetBool(string key, bool value) {
		if (_event == null) return this;
		Span<byte> keyUtf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* k = keyUtf8)
			NativeInterop.GameEventSetBool(_event, k, value ? (byte)1 : (byte)0);
		return this;
	}

	/// <summary>Fires the event. After firing, the event is owned by the engine and must not be used.</summary>
	public bool Fire(bool dontBroadcast = false) {
		if (_event == null || _fired) return false;
		_fired = true;
		var result = NativeInterop.FireGameEvent(_event, dontBroadcast ? (byte)1 : (byte)0);
		_event = null;
		return result != 0;
	}

	public void Dispose() {
		if (_event != null && !_fired) {
			NativeInterop.FreeGameEvent(_event);
			_event = null;
		}
	}
}
