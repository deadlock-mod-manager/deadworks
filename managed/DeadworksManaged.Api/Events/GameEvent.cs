using System.Runtime.InteropServices;

namespace DeadworksManaged.Api;

/// <summary>Represents a Source 2 game event fired by the engine. Read field values via Get* methods; write via Set* methods.</summary>
public unsafe class GameEvent {
	internal nint Handle;
	internal GameEvent(nint handle) => Handle = handle;

	/// <summary>The event name (e.g. "player_death").</summary>
	public string Name { get; internal set; } = "";

	/// <summary>Gets a bool field from this event.</summary>
	public bool GetBool(string key, bool def = false) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			return NativeInterop.GameEventGetBool((void*)Handle, ptr, def ? (byte)1 : (byte)0) != 0;
		}
	}

	/// <summary>Gets an int field from this event.</summary>
	public int GetInt(string key, int def = 0) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			return NativeInterop.GameEventGetInt((void*)Handle, ptr, def);
		}
	}

	/// <summary>Gets a float field from this event.</summary>
	public float GetFloat(string key, float def = 0f) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			return NativeInterop.GameEventGetFloat((void*)Handle, ptr, def);
		}
	}

	/// <summary>Gets a string field from this event.</summary>
	public string GetString(string key, string def = "") {
		Span<byte> keyUtf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		Span<byte> defUtf8 = Utf8.Encode(def, stackalloc byte[Utf8.Size(def)]);
		fixed (byte* keyPtr = keyUtf8, defPtr = defUtf8) {
			byte* result = NativeInterop.GameEventGetString((void*)Handle, keyPtr, defPtr);
			return Marshal.PtrToStringUTF8((nint)result) ?? def;
		}
	}

	/// <summary>Sets a bool field on this event.</summary>
	public void SetBool(string key, bool val) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.GameEventSetBool((void*)Handle, ptr, val ? (byte)1 : (byte)0);
		}
	}

	/// <summary>Sets an int field on this event.</summary>
	public void SetInt(string key, int val) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.GameEventSetInt((void*)Handle, ptr, val);
		}
	}

	/// <summary>Sets a float field on this event.</summary>
	public void SetFloat(string key, float val) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.GameEventSetFloat((void*)Handle, ptr, val);
		}
	}

	/// <summary>Sets a string field on this event.</summary>
	public void SetString(string key, string val) {
		Span<byte> keyUtf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		Span<byte> valUtf8 = Utf8.Encode(val, stackalloc byte[Utf8.Size(val)]);
		fixed (byte* keyPtr = keyUtf8, valPtr = valUtf8) {
			NativeInterop.GameEventSetString((void*)Handle, keyPtr, valPtr);
		}
	}

	/// <summary>Gets a uint64 field from this event.</summary>
	public ulong GetUint64(string key, ulong def = 0) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			return NativeInterop.GameEventGetUint64((void*)Handle, ptr, def);
		}
	}

	/// <summary>Gets a player controller entity referenced by an ehandle field in this event.</summary>
	public CBasePlayerController? GetPlayerController(string key) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			void* result = NativeInterop.GameEventGetPlayerController((void*)Handle, ptr);
			return result != null ? new CBasePlayerController((nint)result) : null;
		}
	}

	/// <summary>Gets a player pawn entity referenced by an ehandle field in this event.</summary>
	public CBasePlayerPawn? GetPlayerPawn(string key) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			void* result = NativeInterop.GameEventGetPlayerPawn((void*)Handle, ptr);
			return result != null ? new CBasePlayerPawn((nint)result) : null;
		}
	}

	/// <summary>Gets an entity by resolving the entity handle stored in the named event field.</summary>
	public CBaseEntity? GetEHandle(string key) {
		Span<byte> utf8 = Utf8.Encode(key, stackalloc byte[Utf8.Size(key)]);
		fixed (byte* ptr = utf8) {
			uint raw = NativeInterop.GameEventGetEHandle((void*)Handle, ptr);
			if (raw == 0xFFFFFFFF) return null;
			void* entity = NativeInterop.GetEntityFromHandle(raw);
			return entity != null ? new CBaseEntity((nint)entity) : null;
		}
	}
}
