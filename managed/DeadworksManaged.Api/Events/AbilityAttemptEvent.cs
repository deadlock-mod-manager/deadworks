namespace DeadworksManaged.Api;

/// <summary>
/// Event fired before ability execution each think tick.
/// Set <see cref="BlockedButtons"/> to prevent specific abilities from being cast.
/// </summary>
public sealed class AbilityAttemptEvent {
	/// <summary>Player slot (0-63).</summary>
	public required int PlayerSlot { get; init; }

	/// <summary>Current held button state (CInButtonState[0]).</summary>
	public required InputButton HeldButtons { get; init; }

	/// <summary>Changed button state (CInButtonState[1]).</summary>
	public required InputButton ChangedButtons { get; init; }

	/// <summary>Scroll button state (CInButtonState[2]).</summary>
	public required InputButton ScrollButtons { get; init; }

	/// <summary>
	/// Set bits in this mask to block those buttons from being processed by the ability system.
	/// Cleared bits have no effect. Multiple plugins' blocked masks are OR'd together.
	/// </summary>
	public InputButton BlockedButtons { get; set; }

	/// <summary>
	/// Set bits in this mask to force those buttons to be pressed, even if the player isn't pressing them.
	/// Multiple plugins' forced masks are OR'd together.
	/// </summary>
	public InputButton ForcedButtons { get; set; }

	/// <summary>Gets the player controller for this player slot.</summary>
	[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
	public unsafe CCitadelPlayerController? Controller {
		get {
			if (NativeInterop.GetPlayerController == null)
				return null;
			var ptr = NativeInterop.GetPlayerController(PlayerSlot);
			return ptr != null ? new CCitadelPlayerController((nint)ptr) : null;
		}
	}

	/// <summary>True if the given button(s) are held this tick.</summary>
	public bool IsHeld(InputButton button) => (HeldButtons & button) != 0;
	/// <summary>True if the given button(s) changed this tick.</summary>
	public bool IsChanged(InputButton button) => (ChangedButtons & button) != 0;

	public bool IsAnyAbilityHeld => (HeldButtons & InputButton.AllAbilities) != 0;
	public bool IsAnyItemHeld => (HeldButtons & InputButton.AllItems) != 0;

	/// <summary>Block the specified button(s) from being processed.</summary>
	public void Block(InputButton button) => BlockedButtons |= button;
	public void BlockAllAbilities() => BlockedButtons |= InputButton.AllAbilities;
	public void BlockAllItems() => BlockedButtons |= InputButton.AllItems;
	public void BlockAll() => BlockedButtons |= InputButton.AllAbilities | InputButton.AllItems;

	/// <summary>Force the specified button(s) to be pressed.</summary>
	public void Force(InputButton button) => ForcedButtons |= button;
	public void ForceAllAbilities() => ForcedButtons |= InputButton.AllAbilities;
	public void ForceAllItems() => ForcedButtons |= InputButton.AllItems;
}
