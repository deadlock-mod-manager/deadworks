using System.Numerics;

namespace DeadworksManaged.Api;

/// <summary>
/// Displays world text anchored to the player's camera via per-frame teleport.
/// </summary>
public sealed unsafe class ScreenText
{
	private static readonly List<ScreenText> _active = new();

	private CPointWorldText? _textEntity;
	private CCitadelPlayerController? _controller;
	private float _posX;
	private float _posY;
	private float _fwdOffset;

	private const float DefaultForwardOffset = 7f;
	private const float DefaultEyeHeight = 64f;
	private const float Deg2Rad = MathF.PI / 180f;

	private const float ScreenLeft = -9.28f;
	private const float ScreenWidth = 18.5f;
	private const float ScreenBottom = -4.8f;
	private const float ScreenHeight = 10.13f;

	/// <summary>The underlying world text entity, or <see langword="null"/> if creation failed or the text has been destroyed.</summary>
	public CPointWorldText? Entity => _textEntity;

	/// <summary><see langword="true"/> if the text entity and its owning controller are both still alive.</summary>
	public bool IsValid => _textEntity != null && _controller != null;

	/// <summary>
	/// Creates a screen-space text label for a player and begins tracking it each frame.
	/// </summary>
	/// <param name="controller">The player whose camera the text will follow.</param>
	/// <param name="message">The text string to display.</param>
	/// <param name="posX">Horizontal screen position in [0, 1] where 0 is left and 1 is right. Default 0.5 (center).</param>
	/// <param name="posY">Vertical screen position in [0, 1] where 0 is bottom and 1 is top. Default 0.5 (center).</param>
	/// <param name="fontSize">Font size of the world text entity.</param>
	/// <param name="r">Red component of the text color.</param>
	/// <param name="g">Green component of the text color.</param>
	/// <param name="b">Blue component of the text color.</param>
	/// <param name="a">Alpha component of the text color.</param>
	/// <param name="fwdOffset">How far in front of the camera (in world units) the text is placed.</param>
	/// <returns>The new <see cref="ScreenText"/> instance. Check <see cref="IsValid"/> before use.</returns>
	public static ScreenText Create(
		CCitadelPlayerController controller, string message,
		float posX = 0.5f, float posY = 0.5f,
		float fontSize = 24f,
		byte r = 255, byte g = 255, byte b = 255, byte a = 255,
		float fwdOffset = DefaultForwardOffset)
	{
		var st = new ScreenText();
		st._controller = controller;
		st._posX = posX;
		st._posY = posY;
		st._fwdOffset = fwdOffset;

		var pawn = controller.GetHeroPawn();
		if (pawn == null) return st;

		var text = CPointWorldText.Create(message, pawn.Position, fontSize, r, g, b, a);
		if (text == null) return st;
		st._textEntity = text;

		st.UpdatePosition();

		_active.Add(st);
		return st;
	}

	/// <summary>Updates the displayed text string.</summary>
	/// <param name="text">The new message to show.</param>
	public void SetText(string text) => _textEntity?.SetMessage(text);

	/// <summary>Changes the screen-space anchor position for subsequent frames.</summary>
	/// <param name="posX">Horizontal position in [0, 1].</param>
	/// <param name="posY">Vertical position in [0, 1].</param>
	public void SetPosition(float posX, float posY)
	{
		_posX = posX;
		_posY = posY;
	}

	/// <summary>Removes the text entity and stops tracking it. Safe to call multiple times.</summary>
	public void Destroy()
	{
		if (_textEntity != null) { _textEntity.Remove(); _textEntity = null; }
		_controller = null;
		_active.Remove(this);
	}

	/// <summary>Called every game frame.</summary>
	internal static void UpdateAll()
	{
		for (int i = _active.Count - 1; i >= 0; i--)
		{
			var st = _active[i];
			if (!st.IsValid)
			{
				st.Destroy();
				continue;
			}
			st.UpdatePosition();
		}
	}

	private void UpdatePosition()
	{
		if (_textEntity == null || _controller == null) return;
		var pawn = _controller.GetHeroPawn();
		if (pawn == null) return;

		var camAngles = pawn.CameraAngles;
		AngleVectors(camAngles, out var fwd, out var right, out var up);

		var eyePos = pawn.Position;
		eyePos.Z += DefaultEyeHeight;

		float rightOffset = ScreenLeft + (_posX * ScreenWidth);
		float upOffset = ScreenBottom + (_posY * ScreenHeight);

		var textPos = eyePos
			+ fwd * _fwdOffset
			+ right * rightOffset
			+ up * upOffset;

		var textAngles = new Vector3(
			0f,
			camAngles.Y + 270f,
			90f - camAngles.X
		);

		_textEntity.Teleport(position: textPos, angles: textAngles);
	}

	private static void AngleVectors(Vector3 angles, out Vector3 forward, out Vector3 right, out Vector3 up)
	{
		float sp = MathF.Sin(angles.X * Deg2Rad);
		float cp = MathF.Cos(angles.X * Deg2Rad);
		float sy = MathF.Sin(angles.Y * Deg2Rad);
		float cy = MathF.Cos(angles.Y * Deg2Rad);
		float sr = MathF.Sin(angles.Z * Deg2Rad);
		float cr = MathF.Cos(angles.Z * Deg2Rad);

		forward = new Vector3(cp * cy, cp * sy, -sp);
		right = new Vector3(
			-sr * sp * cy + cr * sy,
			-sr * sp * sy - cr * cy,
			-sr * cp
		);
		up = new Vector3(
			cr * sp * cy + sr * sy,
			cr * sp * sy - sr * cy,
			cr * cp
		);
	}
}
