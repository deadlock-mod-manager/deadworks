using System.Runtime.CompilerServices;
using System.Text;

namespace DeadworksManaged.Api;

/// <summary>
/// Helpers for null-terminated UTF-8 string marshalling to native code.
/// <para>Usage: <c>Span&lt;byte&gt; utf8 = Utf8.Encode(str, stackalloc byte[Utf8.Size(str)]);</c></para>
/// </summary>
public static class Utf8
{
	/// <summary>Returns buffer size needed for a null-terminated UTF-8 encoding.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Size(string value) => Encoding.UTF8.GetByteCount(value) + 1;

	/// <summary>Writes null-terminated UTF-8 into <paramref name="buffer"/> and returns it.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> Encode(string value, Span<byte> buffer)
	{
		Encoding.UTF8.GetBytes(value, buffer);
		buffer[^1] = 0;
		return buffer;
	}
}
