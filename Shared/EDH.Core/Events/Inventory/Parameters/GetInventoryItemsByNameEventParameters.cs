using EDH.Core.Entities;

namespace EDH.Core.Events.Inventory.Parameters;

public sealed record GetInventoryItemsByNameEventParameters(string itemName)
{
    public TaskCompletionSource<IEnumerable<InventoryItem>> CompletionSource { get; init; } = new();
}