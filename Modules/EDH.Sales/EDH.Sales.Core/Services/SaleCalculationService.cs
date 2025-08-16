using EDH.Core.Common;
using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;
using EDH.Sales.Core.Services.Interfaces;
using EDH.Sales.Core.ValueObjects;

namespace EDH.Sales.Core.Services;

public sealed class SaleCalculationService : ISaleCalculationService
{
    public Result<SaleLineCalculation> CalculateLine(Money unitPrice, Quantity quantity, Money unitCosts, DiscountSurcharge discountSurcharge)
    {
        try
        {
            return Result<SaleLineCalculation>.Ok(SaleLineCalculation.Calculate(unitPrice, quantity, unitCosts, discountSurcharge));
        }
        catch (DomainValidationException ex)
        {
            return Result<SaleLineCalculation>.Fail(ex.Message); 
        }
    }

    public Result<SaleCalculation> CalculateTotal(IEnumerable<SaleLineCalculation> saleLineCalculations)
    {
        try
        {
            return Result<SaleCalculation>.Ok(SaleCalculation.Calculate(saleLineCalculations));
        }
        catch (DomainValidationException ex)
        {
            return Result<SaleCalculation>.Fail(ex.Message); 
        }
    }

    public Result<SaleLineCalculation> ReconstructSaleLine(Money unitPrice, Quantity quantity, Money costs, Money adjustment, Money profit, Money subtotal,
        Currency currency)
    {
        try
        {
            return Result<SaleLineCalculation>.Ok(SaleLineCalculation.FromCalculatedValues(unitPrice, quantity, costs, adjustment, profit, subtotal, currency));
        }
        catch (DomainValidationException ex)
        {
            return Result<SaleLineCalculation>.Fail(ex.Message);
        }
    }

    public Result HasAvailability(Quantity requested, Quantity available) =>
        requested.IsGreaterThan(available) 
            ? Result.Fail("Not enough items in stock.") 
            : Result.Ok();
}