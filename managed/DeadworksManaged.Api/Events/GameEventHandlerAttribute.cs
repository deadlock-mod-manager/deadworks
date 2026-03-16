namespace DeadworksManaged.Api;

/// <summary>Delegate type for dynamic game event listener callbacks registered via <see cref="GameEvents"/>.</summary>
public delegate HookResult GameEventHandler(GameEvent e);

/// <summary>Marks a method to be auto-registered as a Source 2 game event handler for the named event.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class GameEventHandlerAttribute : Attribute {
	/// <summary>The Source 2 game event name to listen for (e.g. "player_death").</summary>
	public string EventName { get; }
	public GameEventHandlerAttribute(string eventName) => EventName = eventName;
}
