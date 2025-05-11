using System.Collections.ObjectModel;
using System.Globalization;
using EDH.Core.Constants;
using EDH.Core.Extensions;
using EDH.Presentation.Common.ViewModels;
using EDH.Sales.Application.DTOs;
using EDH.Sales.Application.DTOs.RecordSale;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Presentation.UIModels;

namespace EDH.Sales.Presentation.ViewModels;

internal sealed class RecordSaleViewModel : BaseViewModel, INavigationAware
{
    private readonly ISalesService _salesService;
    private readonly IDialogService _dialogService;
    private bool _isNavigationTarget = true;
    private bool _isNavigatingInItemsComboBox;

    public RecordSaleViewModel(ISalesService salesService, IDialogService dialogService)
    {
        _salesService = salesService;
        _dialogService = dialogService;
        SelectedDiscountSurchargeMode = DiscountSurchargeMode[0];
    }
    
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return _isNavigationTarget;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    private string? _itemName;
    public string ItemName
    {
        get => _itemName ?? String.Empty;
        set
        {
            if (!SetProperty(ref _itemName, value)) return;

            if (_isNavigatingInItemsComboBox) return;

            SelectedItem = null;

            if (String.IsNullOrWhiteSpace(value))
            {
                Items = [];
                IsItemsDropdownOpen = false;
                return;
            }

            SearchItemsCommand.Execute();
        }
    }

    private DelegateCommand? _searchItemsCommand;
    public DelegateCommand SearchItemsCommand =>
        _searchItemsCommand ??= new DelegateCommand(ExecuteSearchItemsCommand);

    private async void ExecuteSearchItemsCommand()
    {
        try
        {
            await SetComboBoxInventoryItems(_itemName ?? String.Empty);
        }
        catch (Exception ex)
        {
            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Inventory item search" },
                { "message", $"Unknown error searching for items: {ex.Message}" }
            });
        }
    }

    private async Task SetComboBoxInventoryItems(string itemName)
    {
        var items = await _salesService.GetInventoryItemsByNameAsync(itemName);
        Items = items.ToList();
        if (SelectedItem is null) IsItemsDropdownOpen = true;
    }

    private GetInventoryItemsRecordSaleDto? _selectedItem;
    public GetInventoryItemsRecordSaleDto? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value)) return;
            
            CleanUp();
            
            if (value is null) return;

            _isNavigatingInItemsComboBox = true;
            ItemName = value.Name;
            _isNavigatingInItemsComboBox = false;
            
            ItemQuantity = "1";
            _itemQuantityValue = 1;
            UnitPrice = value.ItemRecordSale.Price.ToString("C2");
            _unitPriceValue = value.ItemRecordSale.Price;
            CalculateLineSubTotals();
        }
    }

    private List<GetInventoryItemsRecordSaleDto>? _items;
    public List<GetInventoryItemsRecordSaleDto> Items
    {
        get => _items ?? [];
        set => SetProperty(ref _items, value);
    }

    private bool _isItemsDropdownOpen;
    public bool IsItemsDropdownOpen
    {
        get => _isItemsDropdownOpen;
        set => SetProperty(ref _isItemsDropdownOpen, value);
    }

    private decimal _unitPriceValue;
    private string _unitPrice = 0.ToString("C2");
    public string UnitPrice
    {
        get => _unitPrice;
        set => SetProperty(ref _unitPrice, value);
    }

    private int _itemQuantityValue;
    private string? _itemQuantity;
    public string ItemQuantity
    {
        get => _itemQuantity ?? String.Empty;
        set
        {
            ValidateAndSetItemQuantity(value);
            
            if (!SetProperty(ref _itemQuantity, value)) return;
            
            CalculateLineSubTotals();
        }
    }

    private void ValidateAndSetItemQuantity(string itemQuantity)
    {
        if (String.IsNullOrWhiteSpace(itemQuantity) && SelectedItem is not null)
        {
            _itemQuantityValue = 0;
            SetError(nameof(ItemQuantity), "Quantity is required");
            return;
        }
        
        if ((!Int32.TryParse(itemQuantity, out int parsedValue) || 
            parsedValue <= 0 ) && 
            SelectedItem is not null)

        {
            _itemQuantityValue = 0;
            SetError(nameof(ItemQuantity), "Only whole positive numeric values allowed");
            return;
        }
        
        _itemQuantityValue = parsedValue;
        ClearError(nameof(ItemQuantity));
    }

    private decimal _itemDiscountOrSurchargeValue;
    private string? _itemDiscountOrSurcharge;
    public string ItemDiscountOrSurcharge
    {
        get => _itemDiscountOrSurcharge ?? String.Empty;
        set
        {
            ValidateAndSetItemDiscountOrSurcharge(value);
            
            if (!SetProperty(ref _itemDiscountOrSurcharge, value)) return;
            
            CalculateLineSubTotals();
        }
    }

    private void ValidateAndSetItemDiscountOrSurcharge(string itemDiscountOrSurcharge)
    {
        if (String.IsNullOrWhiteSpace(itemDiscountOrSurcharge))
        {
            _itemDiscountOrSurchargeValue = 0;
            ClearError(nameof(ItemDiscountOrSurcharge));
            return;
        }

        if (!itemDiscountOrSurcharge.TryToDecimal(out decimal itemDiscountOrSurchargeValue))
        {
            _itemDiscountOrSurchargeValue = 0;
            SetError(nameof(ItemDiscountOrSurcharge), "Only numeric values allowed");
            return;
        }
        
        _itemDiscountOrSurchargeValue = itemDiscountOrSurchargeValue;
        ClearError(nameof(ItemDiscountOrSurcharge));
    }

    private List<string> _discountSurchargeMode = ["%", "$"];
    public List<string> DiscountSurchargeMode   
    {
        get => _discountSurchargeMode;
        set => SetProperty(ref _discountSurchargeMode, value);
    }

    private string? _selectedDiscountSurchargeMode;
    public string SelectedDiscountSurchargeMode
    {
        get => _selectedDiscountSurchargeMode ?? String.Empty;
        set
        {
            if (!SetProperty(ref _selectedDiscountSurchargeMode, value)) return;

            CalculateLineSubTotals();
        }
    }

    private decimal _variableCostsLineValue;
    private string _variableCostsLineSum = 0.ToString("C2");
    public string VariableCostsLineSum
    {
        get => _variableCostsLineSum;
        set => SetProperty(ref _variableCostsLineSum, value);
    }
    
    private string _profit = 0.ToString("C2");
    public string Profit
    {
        get => _profit;
        set => SetProperty(ref _profit, value);
    }

    private decimal _profitValue;
    public decimal ProfitValue
    {
        get => _profitValue;
        set => SetProperty(ref _profitValue, value);
    }

    private decimal _subTotalValue;
    private string _subTotal = 0.ToString("C2");
    public string SubTotal
    {
        get => _subTotal;
        set => SetProperty(ref _subTotal, value);
    }
    
    private ObservableCollection<SaleLineViewModel> _saleLines = [];
    public ObservableCollection<SaleLineViewModel> SaleLines
    {
        get => _saleLines;
        set => SetProperty(ref _saleLines, value);
    }

    private DelegateCommand? _addSaleLineCommand;
    public DelegateCommand AddSaleLineCommand => _addSaleLineCommand ??=
        new DelegateCommand(ExecuteAddSaleLineCommand, CanExecuteAddSaleLineCommand)
            .ObservesProperty(() => SelectedItem)
            .ObservesProperty(() => ItemQuantity)
            .ObservesProperty(() => ItemDiscountOrSurcharge);

    private void ExecuteAddSaleLineCommand()
    {
        var saleLine = new SaleLineViewModel
        {
            ItemId = SelectedItem!.Id,
            ItemName = SelectedItem!.Name,
            UnitPrice = _unitPriceValue.ToString(CultureInfo.CurrentCulture),
            Quantity = _itemQuantityValue.ToString(),
            Costs = _variableCostsLineValue.ToString(CultureInfo.CurrentCulture),
            Adjustment = SelectedDiscountSurchargeMode.Equals("$") 
                ? _itemDiscountOrSurchargeValue
                : (_itemDiscountOrSurchargeValue / 100) * (_unitPriceValue * _itemQuantityValue),
            Profit = ProfitValue,
            Subtotal = _subTotalValue
        };
        
        SaleLines.Add(saleLine);
        SelectedItem = null;
    }

    private bool CanExecuteAddSaleLineCommand() =>
        SelectedItem is not null &&
        !String.IsNullOrWhiteSpace(ItemQuantity) &&
        Int32.TryParse(ItemQuantity, out int itemQuantityParsed) &&
        itemQuantityParsed > 0 &&
        (String.IsNullOrWhiteSpace(ItemDiscountOrSurcharge) || ItemDiscountOrSurcharge.TryToDecimal(out _));
    
    private void CalculateLineSubTotals()
    {
        if (SelectedItem is null) return;

        CalculateLineSubTotal();
        
        CalculateLineVariableCosts();

        CalculateLineProfit();
    }

    private void CalculateLineSubTotal()
    {
        _subTotalValue = SelectedItem!.ItemRecordSale.Price * _itemQuantityValue;
        
        if (SelectedDiscountSurchargeMode.Equals("$"))
        {
            _subTotalValue += _itemDiscountOrSurchargeValue;
        }
        else
        {
            _subTotalValue += (_subTotalValue * (_itemDiscountOrSurchargeValue / 100m));
        }
        
        SubTotal = _subTotalValue.ToString("C2");
    }

    private void CalculateLineProfit()
    {
        ProfitValue = _subTotalValue - _variableCostsLineValue;
        Profit = ProfitValue.ToString("C2");
    }

    private void CalculateLineVariableCosts()
    {
        _variableCostsLineValue = SelectedItem!.ItemRecordSale.VariableCost * _itemQuantityValue;
        VariableCostsLineSum = _variableCostsLineValue.ToString("C2");
    }

    private void CleanUp()
    {
        _unitPriceValue = 0;
        UnitPrice = 0.ToString("C2");
        _itemQuantityValue = 0;
        ItemQuantity = String.Empty;
        _itemDiscountOrSurchargeValue = 0;
        ItemDiscountOrSurcharge = String.Empty;
        _variableCostsLineValue = 0;
        VariableCostsLineSum = 0.ToString("C2");
        ProfitValue = 0;
        Profit = 0.ToString("C2");
        _subTotalValue = 0;
        SubTotal = 0.ToString("C2");
    }
}