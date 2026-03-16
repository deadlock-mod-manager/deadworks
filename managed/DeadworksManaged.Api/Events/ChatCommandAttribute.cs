namespace DeadworksManaged.Api;

/// <summary>Marks a plugin method as a handler for a chat command (e.g. <c>"!mycommand"</c>). Can be applied multiple times to map multiple commands to the same method.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ChatCommandAttribute : Attribute
{
    /// <summary>The chat command string this attribute matches, including the prefix (e.g. <c>"!mycommand"</c>).</summary>
    public string Command { get; }

    /// <param name="command">The command string to match, including any prefix character.</param>
    public ChatCommandAttribute(string command) => Command = command;
}
