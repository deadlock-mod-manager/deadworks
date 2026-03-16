using System.Runtime.InteropServices;

namespace DeadworksManaged.Api;

[StructLayout(LayoutKind.Sequential)]
internal struct SchemaFieldResult
{
	public int Offset;
	public short ChainOffset;
	public byte Networked;
	public byte Pad;
}
