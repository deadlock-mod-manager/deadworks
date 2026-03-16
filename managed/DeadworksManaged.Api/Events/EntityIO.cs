namespace DeadworksManaged.Api;

/// <summary>Hooks into the Valve entity I/O system to observe output firings and input dispatches by entity designer name.</summary>
public static class EntityIO {
	internal static Func<string, string, Action<EntityOutputEvent>, IHandle>? OnHookOutput;
	internal static Func<string, string, Action<EntityInputEvent>, IHandle>? OnHookInput;

	/// <summary>Subscribes to outputs fired by entities with the given designer name and output name.</summary>
	/// <param name="designerName">The entity designer name to match (e.g. <c>"trigger_multiple"</c>).</param>
	/// <param name="outputName">The output name to match (e.g. <c>"OnTrigger"</c>).</param>
	/// <param name="handler">Callback invoked each time the output fires.</param>
	/// <returns>An <see cref="IHandle"/> that cancels the hook when disposed or released.</returns>
	public static IHandle HookOutput(string designerName, string outputName, Action<EntityOutputEvent> handler) {
		return OnHookOutput?.Invoke(designerName, outputName, handler) ?? CallbackHandle.Noop;
	}

	/// <summary>Subscribes to inputs dispatched to entities with the given designer name and input name.</summary>
	/// <param name="designerName">The entity designer name to match (e.g. <c>"func_button"</c>).</param>
	/// <param name="inputName">The input name to match (e.g. <c>"Kill"</c>).</param>
	/// <param name="handler">Callback invoked each time the input is dispatched.</param>
	/// <returns>An <see cref="IHandle"/> that cancels the hook when disposed or released.</returns>
	public static IHandle HookInput(string designerName, string inputName, Action<EntityInputEvent> handler) {
		return OnHookInput?.Invoke(designerName, inputName, handler) ?? CallbackHandle.Noop;
	}
}
