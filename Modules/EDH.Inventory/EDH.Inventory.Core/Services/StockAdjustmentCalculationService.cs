using EDH.Core.Common;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;
using EDH.Inventory.Core.Services.Interfaces;
using EDH.Inventory.Core.ValueObjects;

namespace EDH.Inventory.Core.Services;

public sealed class StockAdjustmentCalculationService : IStockAdjustmentCalculationService
{
    public Result<StockAdjustmentCalculation> Calculate(Quantity current, int adjustment)
    {
        try
        {
            return Result<StockAdjustmentCalculation>.Ok(StockAdjustmentCalculation.Calculate(current, adjustment));
        }
        catch (DomainValidationException ex)
        {
            return Result<StockAdjustmentCalculation>.Fail(ex.Message);
        }
    }
}