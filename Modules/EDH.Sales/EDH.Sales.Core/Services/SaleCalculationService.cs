using EDH.Core.Common;
using EDH.Core.ValueObjects;
using EDH.Sales.Core.Services.Interfaces;
using EDH.Sales.Core.ValueObjects;

namespace EDH.Sales.Core.Services;

public sealed class SaleCalculationService : ISaleCalculationService
{
    public Result<SaleLineCalculation> CalculateLine(Money unitPrice, Quantity quantity, Money unitCosts, DiscountSurcharge discountSurcharge)
    {
        if (quantity.IsZero)
            return Result<SaleLineCalculation>.Fail("Quantity must be greater than zero");
        
        return Result<SaleLineCalculation>.Ok(SaleLineCalculation.Calculate(unitPrice, quantity, unitCosts, discountSurcharge));
    }

    public Result<SaleCalculation> CalculateTotal(IEnumerable<SaleLineCalculation> saleLineCalculations) => 
        Result<SaleCalculation>.Ok(SaleCalculation.Calculate(saleLineCalculations));

    public Result HasAvailability(Quantity requested, Quantity available) =>
        requested.IsGreaterThan(available) 
            ? Result.Fail("Not enough items in stock") 
            : Result.Ok();
}