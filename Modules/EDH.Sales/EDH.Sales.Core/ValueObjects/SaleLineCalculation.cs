using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Sales.Core.ValueObjects;

public sealed record SaleLineCalculation
{
    public required Money UnitPrice { get; init; }
    public required Quantity Quantity { get; init; }
    public required Money Costs { get; init; }
    public required Money Adjustment { get; init; }
    public required Money Profit { get; init; }
    public required Money Subtotal { get; init; }
    public required Currency Currency { get; init; }

    public static SaleLineCalculation Calculate(Money unitPrice, Quantity quantity, Money unitCosts, DiscountSurcharge discountSurcharge)
    {
        if (quantity.IsZero)
            throw new DomainValidationException("Quantity must be greater than zero.");
        if (unitPrice < 0)
            throw new DomainValidationException("UnitPrice must be greater than zero.");
        if (unitCosts < 0)
            throw new DomainValidationException("UnitCosts must be greater than zero.");
        
        var currencies = new[] { unitPrice.Currency, unitCosts.Currency };
        
        if (currencies.Any(c => c != currencies[0]))
            throw new InvalidCurrencyException(unitPrice.Currency, unitCosts.Currency);
        
        var grossTotal = unitPrice.MultiplyBy(quantity.Value);
        var subTotal = discountSurcharge.Apply(grossTotal);
        var variableCosts = unitCosts.MultiplyBy(quantity.Value);
        var profit = subTotal.Subtract(variableCosts);
        var adjustment = discountSurcharge.DiscountSurchargeValue(grossTotal);
        
        return new SaleLineCalculation
        {
            UnitPrice = unitPrice,
            Quantity = quantity,
            Costs = variableCosts,
            Adjustment = adjustment,
            Profit = profit,
            Subtotal = subTotal,
            Currency = currencies[0]
        };
    }
}