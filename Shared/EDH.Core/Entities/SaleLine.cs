using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Core.Entities;

public sealed class SaleLine
{
    public int Id { get; set; }
    
    public required int ItemId { get; set; }
    
    private decimal _unitPriceAmount;
    public required Money UnitPrice
    {
        get => Money.FromAmount(_unitPriceAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _unitPriceAmount = value.Amount;
        }
    }
    
    public required Quantity Quantity { get; set; }
    
    private decimal _unitVariableCostsAmount;
    public required Money UnitVariableCosts
    {
        get => Money.FromAmount(_unitVariableCostsAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _unitVariableCostsAmount = value.Amount;
        }
    }
    
    private decimal _totalVariableCostsAmount;
    public required Money TotalVariableCosts
    {
        get => Money.FromAmount(_totalVariableCostsAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _totalVariableCostsAmount = value.Amount;
        }
    }
    
    private decimal _adjustmentAmount;
    public Money Adjustment
    {
        get => Money.FromAmount(_adjustmentAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _adjustmentAmount = value.Amount;
        }
    }
    
    private decimal _profitAmount;
    public required Money Profit
    {
        get => Money.FromAmount(_profitAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _profitAmount = value.Amount;
        }
    }
    
    private decimal _subtotalAmount;
    public required Money Subtotal
    {
        get => Money.FromAmount(_subtotalAmount, Currency);
        set
        {
            if (value.Currency != Currency)
                throw new InvalidCurrencyException(Currency, value.Currency);
            
            _subtotalAmount = value.Amount;
        }
    }
    
    public required Currency Currency { get; set; }
    
    public int SaleId { get; set; }
    
    public Item Item { get; set; } = null!;
    
    public Sale Sale { get; set; } = null!;
}