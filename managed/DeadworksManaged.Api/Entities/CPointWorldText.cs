using System.Numerics;

namespace DeadworksManaged.Api;

/// <summary>Wraps the point_worldtext entity — a world-space text panel rendered in 3D.</summary>
[NativeClass("CPointWorldText")]
public sealed unsafe class CPointWorldText : CBaseEntity {
	internal CPointWorldText(nint handle) : base(handle) { }

	private static readonly SchemaAccessor<bool> _enabled = new("CPointWorldText"u8, "m_bEnabled"u8);
	private static readonly SchemaAccessor<bool> _fullbright = new("CPointWorldText"u8, "m_bFullbright"u8);
	private static readonly SchemaAccessor<float> _fontSize = new("CPointWorldText"u8, "m_flFontSize"u8);
	private static readonly SchemaAccessor<float> _worldUnitsPerPx = new("CPointWorldText"u8, "m_flWorldUnitsPerPx"u8);
	private static readonly SchemaAccessor<float> _depthOffset = new("CPointWorldText"u8, "m_flDepthOffset"u8);
	private static readonly SchemaAccessor<uint> _color = new("CPointWorldText"u8, "m_Color"u8);

	public bool Enabled { get => _enabled.Get(Handle); set => _enabled.Set(Handle, value); }
	public bool Fullbright { get => _fullbright.Get(Handle); set => _fullbright.Set(Handle, value); }
	public float FontSize { get => _fontSize.Get(Handle); set => _fontSize.Set(Handle, value); }
	public float WorldUnitsPerPx { get => _worldUnitsPerPx.Get(Handle); set => _worldUnitsPerPx.Set(Handle, value); }
	public float DepthOffset { get => _depthOffset.Get(Handle); set => _depthOffset.Set(Handle, value); }
	public uint Color { get => _color.Get(Handle); set => _color.Set(Handle, value); }

	/// <summary>Sets the message text via entity input.</summary>
	public void SetMessage(string text) => AcceptInput("SetMessage", value: text);

	/// <summary>Creates and spawns a CPointWorldText at the given position.</summary>
	public static CPointWorldText? Create(string message, Vector3 position, float fontSize = 100f, byte r = 255, byte g = 255, byte b = 255, byte a = 255, int reorientMode = 0) {
		var baseEntity = CreateByName("point_worldtext");
		if (baseEntity == null) return null;

		var wt = new CPointWorldText(baseEntity.Handle);

		void* ekv = NativeInterop.CreateEntityKeyValues();
		Span<byte> msgVal = Utf8.Encode(message, stackalloc byte[Utf8.Size(message)]);

		fixed (byte* enabledKey = "enabled\0"u8.ToArray(),
		             fullbrightKey = "fullbright\0"u8.ToArray(),
		             fontSizeKey = "font_size\0"u8.ToArray(),
		             wuppKey = "world_units_per_pixel\0"u8.ToArray(),
		             colorKey = "color\0"u8.ToArray(),
		             justifyHKey = "justify_horizontal\0"u8.ToArray(),
		             justifyVKey = "justify_vertical\0"u8.ToArray(),
		             reorientKey = "reorient_mode\0"u8.ToArray(),
		             msgValPtr = msgVal,
		             msgTextKey = "message_text\0"u8.ToArray()) {
			NativeInterop.EKVSetString(ekv, msgTextKey, msgValPtr);
			NativeInterop.EKVSetBool(ekv, enabledKey, 1);
			NativeInterop.EKVSetInt(ekv, fullbrightKey, 1);
			NativeInterop.EKVSetFloat(ekv, fontSizeKey, fontSize);
			NativeInterop.EKVSetFloat(ekv, wuppKey, (0.25f / 1050f) * fontSize);
			NativeInterop.EKVSetColor(ekv, colorKey, r, g, b, a);
			NativeInterop.EKVSetInt(ekv, justifyHKey, 0);
			NativeInterop.EKVSetInt(ekv, justifyVKey, 0);
			NativeInterop.EKVSetInt(ekv, reorientKey, reorientMode);
		}

		wt.Teleport(position: position);
		wt.Spawn(ekv);

		// Fire Enable + SetMessage inputs after spawn (like SwiftlyS2)
		wt.AcceptInput("SetMessage", value: message);
		wt.AcceptInput("Enable");

		return wt;
	}
}
