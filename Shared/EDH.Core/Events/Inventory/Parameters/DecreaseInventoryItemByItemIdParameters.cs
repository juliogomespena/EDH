using EDH.Core.Common;
using EDH.Core.Entities;

namespace EDH.Core.Events.Inventory.Parameters;

public sealed record DecreaseInventoryItemByItemIdParameters(int ItemId, int Amount)
{
    public required TaskCompletionSource<Result> CompletionSource { get; init; }
}