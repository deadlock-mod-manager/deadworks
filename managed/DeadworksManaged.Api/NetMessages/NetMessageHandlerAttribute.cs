namespace DeadworksManaged.Api;

/// <summary>Marks a method as a handler for one or more net message types. Applied alongside <see cref="NetMessages.HookIncoming{T}"/> or <see cref="NetMessages.HookOutgoing{T}"/> registrations.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class NetMessageHandlerAttribute : Attribute
{
}
