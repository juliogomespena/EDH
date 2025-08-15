using EDH.Core.Common;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;
using EDH.Items.Core.Services.Interfaces;
using EDH.Items.Core.ValueObjects;

namespace EDH.Items.Core.Services;

public sealed class ProfitMarginCalculationService : IProfitMarginCalculationService
{
    public Result<ItemProfitMarginCalculation> CalculateProfitMargin(Money price, Money costs)
    {
        if (price.Amount < 0 || costs.Amount < 0)
            return Result<ItemProfitMarginCalculation>.Fail("Price and costs must be greater or equal to 0.");
        
        return Result<ItemProfitMarginCalculation>.Ok(ItemProfitMarginCalculation.Calculate(price, costs));
    }
}