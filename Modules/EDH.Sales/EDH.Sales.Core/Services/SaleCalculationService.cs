using EDH.Core.Common;
using EDH.Core.Enums;
using EDH.Core.ValueObjects;
using EDH.Sales.Core.Services.Interfaces;
using EDH.Sales.Core.ValueObjects;

namespace EDH.Sales.Core.Services;

public sealed class SaleCalculationService : ISaleCalculationService
{
    public Result<SaleLineCalculation> CalculateLine(Money unitPrice, Quantity quantity, Money unitCosts, DiscountSurcharge discountSurcharge)
    {
        var errors = new List<string>(4);
        
        if (quantity.IsZero)
            errors.Add("Quantity must be greater than zero.");
        if (unitPrice < 0)
            errors.Add("UnitPrice must be greater than zero.");
        if (unitCosts < 0)
            errors.Add("UnitCosts must be greater than zero.");
        
        return errors.Any() 
            ? Result<SaleLineCalculation>.Fail(errors.ToArray()) 
            : Result<SaleLineCalculation>.Ok(SaleLineCalculation.Calculate(unitPrice, quantity, unitCosts, discountSurcharge));
    }

    public Result<SaleCalculation> CalculateTotal(IEnumerable<SaleLineCalculation> saleLineCalculations)
    {
        var saleLines = saleLineCalculations.ToList();
        var errors = new List<string>(7);
        
        if (saleLines.Any(sl => sl.Quantity < 0))
            errors.Add("Quantity must be greater than zero.");
        if (saleLines.Any(sl => sl.Costs < 0))
            errors.Add("Costs must be greater than zero.");
        if (saleLines.Any(sl => sl.UnitPrice < 0))
            errors.Add("UnitPrice must be greater than zero.");

        return errors.Any()
            ? Result<SaleCalculation>.Fail(errors.ToArray())
            : Result<SaleCalculation>.Ok(SaleCalculation.Calculate(saleLines));
    }

    public Result HasAvailability(Quantity requested, Quantity available) =>
        requested.IsGreaterThan(available) 
            ? Result.Fail("Not enough items in stock.") 
            : Result.Ok();
}