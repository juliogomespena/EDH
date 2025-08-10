using EDH.Core.Common;
using EDH.Core.Entities;

namespace EDH.Core.Events.Inventory.Parameters;

public sealed record CreateInventoryItemEventParameters(int ItemId, int? InitialStock, int? StockAlertThreshold)
{
	public TaskCompletionSource<Result<InventoryItem>> CompletionSource { get; init; } = new();
}