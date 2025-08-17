using EDH.Core.Common;
using EDH.Core.ValueObjects;
using EDH.Inventory.Core.ValueObjects;

namespace EDH.Inventory.Core.Services.Interfaces;

public interface IStockAdjustmentCalculationService
{
    Result<StockAdjustmentCalculation> Calculate(Quantity current, int adjustment);
}