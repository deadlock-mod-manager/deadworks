namespace DeadworksManaged.Api;

/// <summary>
/// Marks a property as a console variable. Supports int, float, bool, and string.
/// Typing the name in console prints the current value; typing with an argument sets it.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ConVarAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; set; } = "";
    public bool ServerOnly { get; set; }

    public ConVarAttribute(string name) => Name = name;
}
