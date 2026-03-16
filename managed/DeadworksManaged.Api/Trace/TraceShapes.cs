using System.Numerics;
using System.Runtime.InteropServices;

namespace DeadworksManaged.Api;

/// <summary>Shape data for a line (ray) trace with an optional radius for a swept sphere.</summary>
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct LineTrace {
	public Vector3 StartOffset;
	public float Radius;
}

/// <summary>Shape data for a sphere trace at a fixed center point.</summary>
[StructLayout(LayoutKind.Sequential, Size = 16)]
public struct SphereTrace {
	public Vector3 Center;
	public float Radius;
}

/// <summary>Shape data for an AABB hull trace swept along a ray.</summary>
[StructLayout(LayoutKind.Sequential, Size = 24)]
public struct HullTrace {
	public Vector3 Mins;
	public Vector3 Maxs;
}

/// <summary>Shape data for a capsule trace between two center points with a radius.</summary>
[StructLayout(LayoutKind.Sequential, Size = 28)]
public struct CapsuleTrace {
	public Vector3 Center0;
	public Vector3 Center1;
	public float Radius;
}

/// <summary>Shape data for a convex mesh trace with bounds and a vertex array.</summary>
[StructLayout(LayoutKind.Sequential, Size = 36)]
public unsafe struct MeshTrace {
	public Vector3 Mins;
	public Vector3 Maxs;
	public Vector3* Vertices;
	public int NumVertices;
}

/// <summary>
/// Union ray descriptor for VPhys2 trace queries. Set the active shape via one of the <c>Init</c> overloads
/// or the static factory methods. The <see cref="Type"/> field indicates which union member is valid.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct Ray_t {
	[FieldOffset(0x0)] public LineTrace Line;
	[FieldOffset(0x0)] public SphereTrace Sphere;
	[FieldOffset(0x0)] public HullTrace Hull;
	[FieldOffset(0x0)] public CapsuleTrace Capsule;
	[FieldOffset(0x0)] public MeshTrace Mesh;
	[FieldOffset(0x28)] public RayType_t Type;

	public void Init(Vector3 startOffset) {
		Line.StartOffset = startOffset;
		Line.Radius = 0.0f;
		Type = RayType_t.Line;
	}

	public void Init(Vector3 center, float radius) {
		if (radius > 0.0f) {
			Sphere.Center = center;
			Sphere.Radius = radius;
			Type = RayType_t.Sphere;
		} else {
			Init(center);
		}
	}

	public void Init(Vector3 mins, Vector3 maxs) {
		if (mins != maxs) {
			Hull.Mins = mins;
			Hull.Maxs = maxs;
			Type = RayType_t.Hull;
		} else {
			Init(mins);
		}
	}

	public void Init(Vector3 centerA, Vector3 centerB, float radius) {
		if (centerA != centerB) {
			if (radius > 0.0f) {
				Capsule.Center0 = centerA;
				Capsule.Center1 = centerB;
				Capsule.Radius = radius;
				Type = RayType_t.Capsule;
			} else {
				Init(centerA, centerB);
			}
		} else {
			Init(centerA, radius);
		}
	}

	public unsafe void Init(Vector3 mins, Vector3 maxs, Vector3* vertices, int numVertices) {
		Mesh.Mins = mins;
		Mesh.Maxs = maxs;
		Mesh.Vertices = vertices;
		Mesh.NumVertices = numVertices;
		Type = RayType_t.Mesh;
	}

	public static Ray_t CreateLine(Vector3 startOffset = default) {
		var ray = new Ray_t();
		ray.Init(startOffset);
		return ray;
	}
}

/// <summary>Axis-aligned bounding box with min/max corners.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct BBox_t {
	public Vector3 Mins;
	public Vector3 Maxs;
}

/// <summary>Position + orientation transform (translation and quaternion rotation).</summary>
[StructLayout(LayoutKind.Explicit)]
public struct CTransform {
	[FieldOffset(0x00)] public Vector3 Position;
	[FieldOffset(0x0C)] private int _pad;
	[FieldOffset(0x10)] public Quaternion Orientation;
}
