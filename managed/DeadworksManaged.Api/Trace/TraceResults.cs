using System.Numerics;
using System.Runtime.InteropServices;

namespace DeadworksManaged.Api;

/// <summary>Collision attributes of a physics body: interaction layer masks, entity/owner IDs, and collision group.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct RnCollisionAttr_t {
	public ulong InteractsAs;
	public ulong InteractsWith;
	public ulong InteractsExclude;
	public uint EntityId;
	public uint OwnerId;
	public ushort HierarchyId;
	public CollisionGroup CollisionGroup;
	public CollisionFunctionMask_t CollisionFunctionMask;
}

/// <summary>
/// Query attributes for a VPhys2 shape trace: which layers and object sets to consider,
/// entities to skip, and flags like <see cref="HitSolid"/> and <see cref="HitTrigger"/>.
/// Default-constructed with sensible defaults (hit solid, ignore disabled pairs, all object sets).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct RnQueryShapeAttr_t {
	public MaskTrace InteractsWith;
	public MaskTrace InteractsExclude;
	public MaskTrace InteractsAs;
	public fixed uint EntityIdsToIgnore[2];
	public fixed uint OwnerIdsToIgnore[2];
	public fixed ushort HierarchyIds[2];
	public ushort IncludedDetailLayers;
	public byte TargetDetailLayer;
	public RnQueryObjectSet ObjectSetMask;
	public CollisionGroup CollisionGroup;
	private byte _data;

	public bool HitSolid {
		get => (_data & (1 << 0)) != 0;
		set { if (value) _data |= 1 << 0; else _data &= unchecked((byte)~(1 << 0)); }
	}
	public bool HitSolidRequiresGenerateContacts {
		get => (_data & (1 << 1)) != 0;
		set { if (value) _data |= 1 << 1; else _data &= unchecked((byte)~(1 << 1)); }
	}
	public bool HitTrigger {
		get => (_data & (1 << 2)) != 0;
		set { if (value) _data |= 1 << 2; else _data &= unchecked((byte)~(1 << 2)); }
	}
	public bool ShouldIgnoreDisabledPairs {
		get => (_data & (1 << 3)) != 0;
		set { if (value) _data |= 1 << 3; else _data &= unchecked((byte)~(1 << 3)); }
	}
	public bool IgnoreIfBothInteractWithHitboxes {
		get => (_data & (1 << 4)) != 0;
		set { if (value) _data |= 1 << 4; else _data &= unchecked((byte)~(1 << 4)); }
	}
	public bool ForceHitEverything {
		get => (_data & (1 << 5)) != 0;
		set { if (value) _data |= 1 << 5; else _data &= unchecked((byte)~(1 << 5)); }
	}
	public bool Unknown {
		get => (_data & (1 << 6)) != 0;
		set { if (value) _data |= 1 << 6; else _data &= unchecked((byte)~(1 << 6)); }
	}

	public RnQueryShapeAttr_t() {
		InteractsWith = 0;
		InteractsExclude = 0;
		InteractsAs = 0;
		EntityIdsToIgnore[0] = uint.MaxValue;
		EntityIdsToIgnore[1] = uint.MaxValue;
		OwnerIdsToIgnore[0] = uint.MaxValue;
		OwnerIdsToIgnore[1] = uint.MaxValue;
		HierarchyIds[0] = 0;
		HierarchyIds[1] = 0;
		IncludedDetailLayers = ushort.MaxValue;
		TargetDetailLayer = 0;
		ObjectSetMask = RnQueryObjectSet.All;
		CollisionGroup = CollisionGroup.Always;
		HitSolid = true;
		ShouldIgnoreDisabledPairs = true;
		Unknown = true;
	}
}

/// <summary>
/// Trace filter passed to VPhys2 TraceShape. Embeds a vtable pointer (managed vtable with a destructor and ShouldHitEntity callback)
/// and a <see cref="RnQueryShapeAttr_t"/>. Construct with <c>new CTraceFilter()</c> for entity-aware filtering.
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 8, Size = 72)]
public struct CTraceFilter {
	[FieldOffset(0x0)] private nint _vtable;
	[FieldOffset(0x8)] public RnQueryShapeAttr_t QueryShapeAttributes;
	[FieldOffset(0x40)] public bool IterateEntities;

	public CTraceFilter() {
		_vtable = CTraceFilterVTable.WithEntityFilter;
		QueryShapeAttributes = new RnQueryShapeAttr_t();
	}

	public CTraceFilter(bool checkIgnoredEntities) {
		_vtable = checkIgnoredEntities ? CTraceFilterVTable.WithEntityFilter : CTraceFilterVTable.Simple;
		QueryShapeAttributes = new RnQueryShapeAttr_t();
	}

	internal void EnsureValid() {
		if (_vtable == 0)
			_vtable = CTraceFilterVTable.WithEntityFilter;
	}
}

internal static class CTraceFilterVTable {
	public static readonly nint Simple;
	public static readonly nint WithEntityFilter;

	[UnmanagedCallersOnly]
	private static unsafe void Destructor(CTraceFilter* filter, byte unk) { }

	[UnmanagedCallersOnly]
	private static unsafe byte ShouldHitEntitySimple() => 1;

	[UnmanagedCallersOnly]
	private static unsafe byte ShouldHitEntity(CTraceFilter* filter, nint entity) {
		if (entity == 0) return 0;
		var ent = new CBaseEntity(entity);
		var idx = (uint)ent.EntityIndex;
		return filter->QueryShapeAttributes.EntityIdsToIgnore[0] != idx &&
		       filter->QueryShapeAttributes.EntityIdsToIgnore[1] != idx ? (byte)1 : (byte)0;
	}

	static unsafe CTraceFilterVTable() {
		Simple = Marshal.AllocHGlobal(sizeof(nint) * 2);
		var vtable = new Span<nint>((void*)Simple, 2);
		vtable[0] = (nint)(delegate* unmanaged<CTraceFilter*, byte, void>)&Destructor;
		vtable[1] = (nint)(delegate* unmanaged<byte>)&ShouldHitEntitySimple;

		WithEntityFilter = Marshal.AllocHGlobal(sizeof(nint) * 2);
		var funcTable = new Span<nint>((void*)WithEntityFilter, 2);
		funcTable[0] = (nint)(delegate* unmanaged<CTraceFilter*, byte, void>)&Destructor;
		funcTable[1] = (nint)(delegate* unmanaged<CTraceFilter*, nint, byte>)&ShouldHitEntity;
	}
}

/// <summary>Result of a VPhys2 shape trace. Check <see cref="DidHit"/> and inspect <see cref="HitPoint"/>, <see cref="HitNormal"/>, and <see cref="HitEntity"/>.</summary>
[StructLayout(LayoutKind.Explicit, Size = 192)]
public unsafe struct CGameTrace {
	[FieldOffset(0x00)] public nint SurfaceProperties;
	[FieldOffset(0x08)] public nint pEntity;
	[FieldOffset(0x10)] public nint HitBox;
	[FieldOffset(0x18)] public nint Body;
	[FieldOffset(0x20)] public nint Shape;
	[FieldOffset(0x28)] public ulong Contents;
	[FieldOffset(0x30)] public fixed byte BodyTransform[32];
	[FieldOffset(0x50)] public fixed byte ShapeAttributes[40];
	[FieldOffset(0x78)] public Vector3 StartPos;
	[FieldOffset(0x84)] public Vector3 EndPos;
	[FieldOffset(0x90)] public Vector3 HitNormal;
	[FieldOffset(0x9C)] public Vector3 HitPoint;
	[FieldOffset(0xA8)] public float HitOffset;
	[FieldOffset(0xAC)] public float Fraction;
	[FieldOffset(0xB0)] public int Triangle;
	[FieldOffset(0xB4)] public short HitboxBoneIndex;
	[FieldOffset(0xB6)] public RayType_t RayType;
	[FieldOffset(0xB7)] public bool StartInSolid;
	[FieldOffset(0xB8)] public bool ExactHitPoint;

	public readonly bool DidHit => Fraction < 1.0f || StartInSolid;

	public readonly float Distance => Vector3.Distance(EndPos, StartPos);

	public readonly Vector3 Direction {
		get {
			var dir = EndPos - StartPos;
			return dir == Vector3.Zero ? Vector3.Zero : Vector3.Normalize(dir);
		}
	}

	public readonly CBaseEntity? HitEntity =>
		pEntity != 0 ? new CBaseEntity(pEntity) : null;

	public readonly bool HitEntityByDesignerName(string designerName, out CBaseEntity? outEntity, NameMatchType matchType = NameMatchType.StartsWith) {
		outEntity = null;
		if (!DidHit || pEntity == 0) return false;
		var ent = new CBaseEntity(pEntity);
		var name = ent.DesignerName;
		if (name == null) return false;
		var isMatch = matchType switch {
			NameMatchType.Exact => name.Equals(designerName, StringComparison.OrdinalIgnoreCase),
			NameMatchType.StartsWith => name.StartsWith(designerName, StringComparison.OrdinalIgnoreCase),
			NameMatchType.EndsWith => name.EndsWith(designerName, StringComparison.OrdinalIgnoreCase),
			NameMatchType.Contains => name.Contains(designerName, StringComparison.OrdinalIgnoreCase),
			_ => false,
		};
		if (isMatch) outEntity = ent;
		return isMatch;
	}

	public readonly bool HitEntityByDesignerName(string designerName, NameMatchType matchType = NameMatchType.StartsWith) =>
		HitEntityByDesignerName(designerName, out _, matchType);

	public static CGameTrace Create() {
		var trace = new CGameTrace();
		// Identity quaternion (w=1.0) at bodyTransform+0x0C
		*(float*)(trace.BodyTransform + 0x0C) = 1.0f;
		trace.Fraction = 1.0f;
		trace.Triangle = -1;
		trace.HitboxBoneIndex = -1;
		return trace;
	}
}

/// <summary>Simplified trace result returned by <see cref="Trace.Ray"/>. Contains the hit position, fraction, and full <see cref="CGameTrace"/> data.</summary>
public struct TraceResult {
	public Vector3 HitPosition;
	public float Fraction;
	public bool DidHit;
	public CGameTrace Trace;
}
