namespace DeadworksManaged.Api;

/// <summary>Bitmask of player slots that should receive a net message. Each bit position corresponds to a player slot index.</summary>
public struct RecipientFilter
{
	/// <summary>Raw bitmask where bit <c>i</c> indicates slot <c>i</c> is included.</summary>
	public ulong Mask;

	/// <summary>A filter that targets all 64 possible player slots.</summary>
	public static RecipientFilter All => new() { Mask = ulong.MaxValue };

	/// <summary>A filter targeting exactly one player slot.</summary>
	/// <param name="slot">The player slot index (0-based).</param>
	public static RecipientFilter Single(int slot) => new() { Mask = 1UL << slot };

	/// <summary>Adds a player slot to the filter.</summary>
	/// <param name="slot">The player slot index to add.</param>
	public void Add(int slot) => Mask |= 1UL << slot;

	/// <summary>Removes a player slot from the filter.</summary>
	/// <param name="slot">The player slot index to remove.</param>
	public void Remove(int slot) => Mask &= ~(1UL << slot);

	/// <summary>Returns <see langword="true"/> if the given slot is included in this filter.</summary>
	/// <param name="slot">The player slot index to test.</param>
	public readonly bool HasRecipient(int slot) => (Mask & (1UL << slot)) != 0;
}
