using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Inventory.Core.ValueObjects;

public sealed record StockAdjustmentCalculation
{
    public required Quantity Quantity { get; init; }

    private StockAdjustmentCalculation()
    {
    }

    public static StockAdjustmentCalculation Calculate(Quantity current, int adjustment)
    {
        if (adjustment < 0 && current + adjustment < 0)
            throw new InvalidQuantityException($"Updated quantity cant be negative.");

        return new StockAdjustmentCalculation
        {
            Quantity = current.Add(Quantity.FromValue(adjustment))
        };
    }
}