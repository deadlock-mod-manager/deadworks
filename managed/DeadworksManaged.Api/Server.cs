using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DeadworksManaged.Api;

// Native interop structs matching C++ layout (pointers valid only during the call)
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ConVarInfoNative
{
	public byte* Name;
	public byte* TypeName;
	public byte* Value;
	public byte* DefaultValue;
	public byte* Description;
	public ulong Flags;
	public byte* MinValue;   // null when absent
	public byte* MaxValue;   // null when absent
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ConCommandInfoNative
{
	public byte* Name;
	public byte* Description;
	public ulong Flags;
}

// Public API types
public record ConVarEntry(
	string Name, string Type, string Value, string DefaultValue,
	string Description, ulong Flags, string? Min, string? Max);

public record ConCommandEntry(string Name, string Description, ulong Flags);

/// <summary>Server-side utilities for sending commands to player clients.</summary>
public static unsafe class Server {
	/// <summary>The current map name, set when the server starts up.</summary>
	public static string MapName { get; internal set; } = "";

	/// <summary>Sends a console command to the client in the given slot.</summary>
	public static void ClientCommand(int slot, string command) {
		Span<byte> utf8 = Utf8.Encode(command, stackalloc byte[Utf8.Size(command)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.ClientCommand(slot, ptr);
		}
	}

	/// <summary>Executes a command on the server console.</summary>
	public static void ExecuteCommand(string command) {
		Span<byte> utf8 = Utf8.Encode(command, stackalloc byte[Utf8.Size(command)]);
		fixed (byte* ptr = utf8) {
			NativeInterop.ExecuteServerCommand(ptr);
		}
	}

	/// <summary>Enumerates all registered ConVars by index.</summary>
	public static List<ConVarEntry> EnumerateConVars() {
		var list = new List<ConVarEntry>();
		ConVarInfoNative info;
		for (ushort i = 0; NativeInterop.GetConVarAt(i, &info) != 0; i++) {
			list.Add(new ConVarEntry(
				Utf8Str(info.Name), Utf8Str(info.TypeName), Utf8Str(info.Value), Utf8Str(info.DefaultValue),
				Utf8Str(info.Description), info.Flags,
				info.MinValue != null ? Utf8Str(info.MinValue) : null,
				info.MaxValue != null ? Utf8Str(info.MaxValue) : null));
		}
		return list;
	}

	/// <summary>Enumerates all registered ConCommands by index.</summary>
	public static List<ConCommandEntry> EnumerateConCommands() {
		var list = new List<ConCommandEntry>();
		ConCommandInfoNative info;
		for (ushort i = 0; NativeInterop.GetConCommandAt(i, &info) != 0; i++) {
			list.Add(new ConCommandEntry(Utf8Str(info.Name), Utf8Str(info.Description), info.Flags));
		}
		return list;
	}

	private static string Utf8Str(byte* ptr) =>
		ptr != null ? Marshal.PtrToStringUTF8((nint)ptr) ?? "" : "";
}
