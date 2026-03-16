namespace DeadworksManaged.Api;

/// <summary>Return value for hooks and event handlers. <see cref="Stop"/> blocks further processing; <see cref="Handled"/> signals the event was consumed.</summary>
public enum HookResult { Continue = 0, Stop = 1, Handled = 2 }
