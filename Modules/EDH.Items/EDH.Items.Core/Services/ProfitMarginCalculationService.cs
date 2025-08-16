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
        try
        {
            return Result<ItemProfitMarginCalculation>.Ok(ItemProfitMarginCalculation.Calculate(price, costs));
        }
        catch (DomainValidationException ex)
        {
            return Result<ItemProfitMarginCalculation>.Fail(ex.Message);
        }
    }
}