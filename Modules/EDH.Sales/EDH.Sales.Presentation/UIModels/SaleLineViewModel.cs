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

    private decimal _unitPrice;
    public decimal UnitPrice
    {
        get => _unitPrice;
        set => SetProperty(ref _unitPrice, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
       
    }
    
    private decimal _costs;
    public decimal Costs
    {
        get => _costs;
        set => SetProperty(ref _costs, value);
    }

    private decimal _adjustment;
    public decimal Adjustment
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