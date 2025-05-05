using EDH.Core.Entities;

namespace EDH.Core.Events.Inventory.Parameters;

public sealed record GetInventoryItemsByNameEventParameters(string ItemName)
{
    public TaskCompletionSource<IEnumerable<InventoryItem>> CompletionSource { get; init; } = new();
}