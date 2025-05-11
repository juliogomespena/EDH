using EDH.Core.Extensions;
using EDH.Presentation.Common.ViewModels;

namespace EDH.Sales.Presentation.UIModels;

internal sealed class SaleLineViewModel : BaseViewModel
{
    private int _itemId;
    public int ItemId
    {
        get => _itemId;
        set => SetProperty(ref _itemId, value);
    }

    private string? _itemName;
    public string ItemName
    {
        get => _itemName ?? String.Empty;
        set => SetProperty(ref _itemName, value);
    }

    private decimal _unitPriceValue;
    private string? _unitPrice;
    public string UnitPrice
    {
        get => _unitPriceValue.ToString("C2");
        set
        {
            if (!SetProperty(ref _unitPrice, value)) return;

            _unitPriceValue = !String.IsNullOrWhiteSpace(_unitPrice) 
                ? _unitPrice.ToDecimal()
                : 0;
        }
    }

    private int _quantityValue;
    private string? _quantity;
    public string Quantity
    {
        get => _quantity ?? String.Empty;
        set
        {
            ValidateAndSetQuantity(value);
            
            if (!SetProperty(ref _quantity, value)) return;
        }
    }
    
    private void ValidateAndSetQuantity(string quantity)
    {
        if (String.IsNullOrWhiteSpace(quantity) || 
            (!Int32.TryParse(quantity, out int parsedValue) || 
             parsedValue <= 0 ))
        {
            _quantityValue = 0;
            SetError(nameof(Quantity), String.Empty);
            return;
        }

        _quantityValue = parsedValue;
        ClearError(nameof(Quantity));
    }

    private decimal _costsValue;
    private string? _costs;
    public string Costs
    {
        get => _costsValue.ToString("C2");
        set
        {
            ValidateAndSetCosts(value);
            
            if (!SetProperty(ref _costs, value)) return;
        }
    }

    private void ValidateAndSetCosts(string costs)
    {
        costs = new string(costs.Where(c => Char.IsDigit(c) || c == '.' || c == ',').ToArray());

        if (String.IsNullOrWhiteSpace(costs) || 
            (!costs.TryToDecimal(out decimal parsedValue) || 
             parsedValue <= 0 ))
        {
            _costsValue = 0;
            SetError(nameof(Costs), String.Empty);
            return;
        }

        _costsValue = parsedValue;
        ClearError(nameof(Costs));
    }

    private decimal? _adjustment;
    public decimal? Adjustment
    {
        get => _adjustment;
        set => SetProperty(ref _adjustment, value);
    }

    private decimal _profit;
    public decimal Profit
    {
        get => _profit;
        set => SetProperty(ref _profit, value);
    }

    private decimal _subtotal;
    public decimal Subtotal
    {
        get => _subtotal;
        set => SetProperty(ref _subtotal, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}