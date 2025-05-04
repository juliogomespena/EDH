namespace EDH.Core.Events.Inventory.Parameters;

public sealed record CreateInventoryItemEventParameters(int ItemId, int? InitialStock, int? StockAlertThreshold)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new();
}