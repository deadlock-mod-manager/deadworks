namespace DeadworksManaged.Api;

/// <summary>Wraps a Source 2 console variable (cvar). Use <see cref="Find"/> to look up existing cvars or <see cref="Create"/> to register new ones.</summary>
public sealed unsafe class ConVar {
	private readonly ulong _handle;

	private ConVar(ulong handle) => _handle = handle;

	public bool IsValid => _handle != 0;

	/// <summary>Looks up an existing ConVar by name. Returns null if not found.</summary>
	public static ConVar? Find(string name) {
		Span<byte> utf8 = Utf8.Encode(name, stackalloc byte[Utf8.Size(name)]);
		fixed (byte* ptr = utf8) {
			ulong handle = NativeInterop.FindConVar(ptr);
			return handle != 0 ? new ConVar(handle) : null;
		}
	}

	/// <summary>Creates a new ConVar registered with the engine. Returns null if creation fails.</summary>
	public static ConVar? Create(string name, string defaultValue, string description = "", bool serverOnly = false) {
		Span<byte> nameUtf8 = Utf8.Encode(name, stackalloc byte[Utf8.Size(name)]);
		Span<byte> defUtf8 = Utf8.Encode(defaultValue, stackalloc byte[Utf8.Size(defaultValue)]);
		Span<byte> descUtf8 = Utf8.Encode(description, stackalloc byte[Utf8.Size(description)]);

		ulong flags = serverOnly ? 0x200UL : 0UL; // FCVAR_GAMEDLL

		fixed (byte* namePtr = nameUtf8, defPtr = defUtf8, descPtr = descUtf8) {
			ulong handle = NativeInterop.CreateConVar(namePtr, defPtr, descPtr, flags);
			return handle != 0 ? new ConVar(handle) : null;
		}
	}

	/// <summary>Sets the cvar's value as an integer.</summary>
	public void SetInt(int value) => NativeInterop.SetConVarInt(_handle, value);
	/// <summary>Sets the cvar's value as a float.</summary>
	public void SetFloat(float value) => NativeInterop.SetConVarFloat(_handle, value);
}

/// <summary>
/// Wraps the native CTakeDamageInfo damage descriptor. Can be owned (created via constructor, must be Disposed)
/// or non-owning (obtained from an OnTakeDamage hook). Exposes attacker, inflictor, ability, damage amount, type, and flags.
/// </summary>
public sealed unsafe class CTakeDamageInfo : IDisposable {
	public nint Handle { get; private set; }
	private readonly bool _owned;

	private CTakeDamageInfo(nint handle, bool owned) {
		Handle = handle;
		_owned = owned;
	}

	/// <summary>Wraps an existing native CTakeDamageInfo pointer (non-owning, e.g. from OnTakeDamage hook).</summary>
	internal static CTakeDamageInfo FromExisting(nint handle) => new(handle, owned: false);

	/// <summary>Creates a new native CTakeDamageInfo. Must be disposed after use.</summary>
	public CTakeDamageInfo(float damage, CBaseEntity? attacker = null, CBaseEntity? inflictor = null, CBaseEntity? ability = null, int damageType = 0) {
		Handle = (nint)NativeInterop.CreateDamageInfo(
			inflictor != null ? (void*)inflictor.Handle : null,
			attacker != null ? (void*)attacker.Handle : null,
			ability != null ? (void*)ability.Handle : null,
			damage, damageType);
		_owned = true;
	}

	public void Dispose() {
		if (_owned && Handle != 0) {
			NativeInterop.DestroyDamageInfo((void*)Handle);
			Handle = 0;
		}
	}

	private static readonly SchemaAccessor<uint> _hInflictor = new("CTakeDamageInfo"u8, "m_hInflictor"u8);
	public CBaseEntity? Inflictor {
		get {
			uint raw = _hInflictor.Get(Handle);
			void* ptr = NativeInterop.GetEntityFromHandle(raw);
			return ptr != null ? new CBaseEntity((nint)ptr) : null;
		}
	}

	private static readonly SchemaAccessor<uint> _hAttacker = new("CTakeDamageInfo"u8, "m_hAttacker"u8);
	public CBaseEntity? Attacker {
		get {
			uint raw = _hAttacker.Get(Handle);
			void* ptr = NativeInterop.GetEntityFromHandle(raw);
			return ptr != null ? new CBaseEntity((nint)ptr) : null;
		}
	}

	private static readonly SchemaAccessor<uint> _hAbility = new("CTakeDamageInfo"u8, "m_hAbility"u8);
	public CBaseEntity? Ability {
		get {
			uint raw = _hAbility.Get(Handle);
			void* ptr = NativeInterop.GetEntityFromHandle(raw);
			return ptr != null ? new CBaseEntity((nint)ptr) : null;
		}
	}

	private static readonly SchemaAccessor<uint> _hOriginator = new("CTakeDamageInfo"u8, "m_hOriginator"u8);
	public CBaseEntity? Originator {
		get {
			uint raw = _hOriginator.Get(Handle);
			void* ptr = NativeInterop.GetEntityFromHandle(raw);
			return ptr != null ? new CBaseEntity((nint)ptr) : null;
		}
	}

	private static readonly SchemaAccessor<float> _damage = new("CTakeDamageInfo"u8, "m_flDamage"u8);
	public float Damage { get => _damage.Get(Handle); set => _damage.Set(Handle, value); }

	private static readonly SchemaAccessor<float> _totalledDamage = new("CTakeDamageInfo"u8, "m_flTotalledDamage"u8);
	public float TotalledDamage { get => _totalledDamage.Get(Handle); set => _totalledDamage.Set(Handle, value); }

	private static readonly SchemaAccessor<int> _damageType = new("CTakeDamageInfo"u8, "m_bitsDamageType"u8);
	public int DamageType { get => _damageType.Get(Handle); set => _damageType.Set(Handle, value); }

	private static readonly SchemaAccessor<ulong> _damageFlags = new("CTakeDamageInfo"u8, "m_nDamageFlags"u8);
	public TakeDamageFlags DamageFlags { get => (TakeDamageFlags)_damageFlags.Get(Handle); set => _damageFlags.Set(Handle, (ulong)value); }
}
