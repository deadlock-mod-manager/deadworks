namespace DeadworksManaged.Api;

/// <summary>Event data passed to <see cref="IDeadworksPlugin.OnModifyCurrency"/>. Contains the pawn, currency type, amount, and source of the change.</summary>
public sealed class ModifyCurrencyEvent {
	public required CCitadelPlayerPawn Pawn { get; init; }
	public required ECurrencyType CurrencyType { get; init; }
	public required int Amount { get; init; }
	public required ECurrencySource Source { get; init; }
	public required bool Silent { get; init; }
	public required bool ForceGain { get; init; }
	public required bool SpendOnly { get; init; }
}
