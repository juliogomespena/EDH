using EDH.Core.Common;
using EDH.Core.Entities;

namespace EDH.Core.Events.Inventory.Parameters;

public sealed record GetInventoryItemByItemIdParameters(int ItemId)
{
    public required TaskCompletionSource<Result<InventoryItem?>> CompletionSource { get; init; }
}