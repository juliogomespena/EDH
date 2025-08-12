using System.Collections.Specialized;
using System.ComponentModel;
using EDH.Core.Constants;
using EDH.Core.Enums;
using EDH.Core.Extensions;
using EDH.Presentation.Common.Collections;
using EDH.Presentation.Common.ViewModels;
using EDH.Sales.Application.DTOs.RecordSale;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Presentation.UIModels;
using Microsoft.Extensions.Logging;

namespace EDH.Sales.Presentation.ViewModels;

internal sealed class RecordSaleViewModel : BaseViewModel, INavigationAware
{
    private readonly ISaleService _saleService;
    private readonly IDialogService _dialogService;
    private readonly IRegionManager _regionManager;
    private readonly ILogger<RecordSaleViewModel> _logger;
    private bool _isNavigationTarget = true;
    private bool _isNavigatingInItemsComboBox;

    public RecordSaleViewModel(ISaleService saleService, IDialogService dialogService, IRegionManager regionManager, ILogger<RecordSaleViewModel> logger)
    {
        _saleService = saleService;
        _dialogService = dialogService;
        _regionManager = regionManager;
        _logger = logger;
        SelectedDiscountSurchargeMode = DiscountSurcharge.Money;
        SaleLines.ItemPropertyChanged += SaleLine_PropertyChanged;
        SaleLines.CollectionChanged += SaleLines_CollectionChanged;

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
            _logger.LogCritical(ex, $"Error in {nameof(ExecuteSearchItemsCommand)}.");
            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Inventory item search" },
                { "message", "Unknown error searching for items" }
            });
        }
    }

    private async Task SetComboBoxInventoryItems(string itemName)
    {
        var items = await _saleService.GetInventoryItemsByNameAsync(itemName);
        
        if (items.IsSuccess)
            Items = items.Value?.ToList() ?? [];
        
        if (SelectedItem is null) IsItemsDropdownOpen = true;
    }

    private GetInventoryItemRecordSaleDto? _selectedItem;
    public GetInventoryItemRecordSaleDto? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value)) return;
            
            CleanUpLine();
            
            if (value is null)
            {
                if (GetErrorCount(nameof(ItemName)) > 0) 
                    ClearError(nameof(ItemName));
                
                return;
            }

            _isNavigatingInItemsComboBox = true;
            ItemName = value.Name;
            _isNavigatingInItemsComboBox = false;
            
            ItemQuantity = "1";
            _itemQuantityValue = 1;
            UnitPrice = value.ItemRecordSale.Price.ToString("C2");
            _unitPriceValue = value.ItemRecordSale.Price;
            CalculateLineSubTotals();

            if (value.Quantity >= 1) return;
            
            SetError(nameof(ItemName), "There is no inventory availability for this item");
        }
    }

    private List<GetInventoryItemRecordSaleDto>? _items;
    public List<GetInventoryItemRecordSaleDto> Items
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

        if (parsedValue > SelectedItem?.Quantity)
        {
            _itemQuantityValue = 0;
            SetError(nameof(ItemQuantity), $"Available in stock: {SelectedItem?.Quantity}");
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

    public List<string> DiscountSurchargeMode => ["$", "%"];
    
    private DiscountSurcharge _selectedDiscountSurchargeMode;
    public DiscountSurcharge SelectedDiscountSurchargeMode
    {
        get => _selectedDiscountSurchargeMode;
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
    
    private NotifyingObservableCollection<SaleLineViewModel> _saleLines = [];
    public NotifyingObservableCollection<SaleLineViewModel> SaleLines
    {
        get => _saleLines;
        set => SetProperty(ref _saleLines, value);
    }

    private DelegateCommand? _addSaleLineCommand;
    public DelegateCommand AddSaleLineCommand => _addSaleLineCommand ??=
        new DelegateCommand(ExecuteAddSaleLineCommand, CanExecuteAddSaleLineCommand)
            .ObservesProperty(() => SelectedItem)
            .ObservesProperty(() => ItemQuantity)
            .ObservesProperty(() => ItemDiscountOrSurcharge)
            .ObservesProperty(() => HasErrors);

    private void ExecuteAddSaleLineCommand()
    {
        var sameItem = SaleLines.FirstOrDefault(sl => sl.ItemId == SelectedItem?.Id);
        
        if (sameItem is not null)
        {
            if (sameItem.Quantity + _itemQuantityValue > SelectedItem?.Quantity)
            {
                SetError(nameof(ItemQuantity), $"Available in stock: {SelectedItem?.Quantity}");
                return;           
            }
            
            sameItem.Quantity += _itemQuantityValue;
            sameItem.Costs += _variableCostsLineValue;
            sameItem.Adjustment += _selectedDiscountSurchargeMode == DiscountSurcharge.Money
                ? _itemDiscountOrSurchargeValue
                : (_itemDiscountOrSurchargeValue / 100m) * (_unitPriceValue * _itemQuantityValue);
            sameItem.Profit += ProfitValue;
            sameItem.Subtotal += _subTotalValue;
            SelectedItem = null;
            CalculateSaleTotal();
            return;
        }
        
        var saleLine = new SaleLineViewModel
        {
            ItemId = SelectedItem!.Id,
            ItemName = SelectedItem!.Name,
            UnitPrice = _unitPriceValue,
            Quantity = _itemQuantityValue,
            Costs = _variableCostsLineValue,
            Adjustment = _selectedDiscountSurchargeMode == DiscountSurcharge.Money 
                ? _itemDiscountOrSurchargeValue
                : (_itemDiscountOrSurchargeValue / 100m) * (_unitPriceValue * _itemQuantityValue),
            Profit = ProfitValue,
            Subtotal = _subTotalValue
        };
        
        SaleLines.Add(saleLine);
        SelectedItem = null;
        CalculateSaleTotal();
    }

    private bool CanExecuteAddSaleLineCommand() =>
        SelectedItem is not null &&
        !String.IsNullOrWhiteSpace(ItemQuantity) &&
        Int32.TryParse(ItemQuantity, out int itemQuantityParsed) &&
        itemQuantityParsed > 0 &&
        (String.IsNullOrWhiteSpace(ItemDiscountOrSurcharge) || ItemDiscountOrSurcharge.TryToDecimal(out _)) &&
        !HasErrors;

    private bool _isAllSaleLinesSelected;
    public bool IsAllSaleLinesSelected
    {
        get => _isAllSaleLinesSelected;
        set
        {
            if (!SetProperty(ref _isAllSaleLinesSelected, value)) return;
            
            foreach (var saleLine in SaleLines)
            {
                saleLine.IsSelected = value;
            }
        }
    }
    
    private decimal _variableCostsTotalValue;
    private string _variableCostsTotal = 0.ToString("C2");
    public string VariableCostsTotal
    {
        get => _variableCostsTotal;
        set => SetProperty(ref _variableCostsTotal, value);
    }
    
    private string _profitTotal = 0.ToString("C2");
    public string ProfitTotal
    {
        get => _profitTotal;
        set => SetProperty(ref _profitTotal, value);
    }

    private decimal _profitTotalValue;
    public decimal ProfitTotalValue
    {
        get => _profitTotalValue;
        set => SetProperty(ref _profitTotalValue, value);
    }

    private decimal _totalValue;
    public decimal TotalValue
    {
        get => _totalValue;
        set => SetProperty(ref _totalValue, value);
    }
    
    private string _total = 0.ToString("C2");
    public string Total
    {
        get => _total;
        set => SetProperty(ref _total, value);
    }
    
    private decimal _adjustmentTotalValue;
    public decimal AdjustmentTotalValue
    {
        get => _adjustmentTotalValue;
        set => SetProperty(ref _adjustmentTotalValue, value);
    }
    
    private string _adjustmentTotal = 0.ToString("C2");
    public string AdjustmentTotal
    {
        get => _adjustmentTotal;
        set => SetProperty(ref _adjustmentTotal, value);
    }

    private DelegateCommand? _createSaleCommand;
    public DelegateCommand CreateSaleCommand => _createSaleCommand ??= 
        new DelegateCommand(ExecuteCreateSaleCommand, CanExecuteCreateSaleCommand)
            .ObservesProperty(() => SaleLines)
            .ObservesProperty(() => TotalValue);

    private async void ExecuteCreateSaleCommand()
    {
        try
        {
            bool shouldProceed = true;
            
            _dialogService.ShowDialog(NavigationConstants.Dialogs.YesNoDialog, new DialogParameters
            {
                { "title", "Sale Registration" },
                {
                    "message",
                    "Confirm sale registration?"
                }
            }, dialogResult =>
            {
                if (dialogResult.Result is ButtonResult.No) shouldProceed = false;
            });
            
            if (!shouldProceed) return;
            
            var saleLines = SaleLines
                .Select(sl => new SaleLineRecordSaleDto(
                    sl.ItemId, sl.ItemName, sl.UnitPrice, sl.Quantity, sl.Costs, sl.Adjustment, sl.Profit, sl.Subtotal
                ));
        
            var sale = new SaleRecordSaleDto(
                0, _variableCostsTotalValue, ProfitTotalValue, AdjustmentTotalValue, TotalValue, saleLines
            );
            
            var result = await _saleService.CreateSaleAsync(sale);

            if (result.IsFailure)
            {
                _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
                {
                    { "title", "Sale Registration" },
                    { "message", $"One or more errors occurred: {String.Join(' ', result.Errors)}" }
                });

                return;
            }
            
            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Sale Registration" },
                { "message", $"Sale {result.Value?.Id} has been registered successfully." }
            });
            
            CleanUpLine();
            CleanUpTotals();
            _isNavigationTarget = false;
            _regionManager.RequestNavigate(NavigationConstants.Regions.MainWindowContent,
                NavigationConstants.Views.RecordSale);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Error in {nameof(ExecuteCreateSaleCommand)}.");
            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Sale Registration" },
                { "message", $"Unknown error occurred" }
            });
        }
    }

    private bool CanExecuteCreateSaleCommand() =>
        SaleLines.Any() &&
        TotalValue > 0;

    private bool _hasSelectedItems;
    public bool HasSelectedItems
    {
        get => _hasSelectedItems;
        set => SetProperty(ref _hasSelectedItems, value);
    }
    
    private DelegateCommand? _deleteSaleLineCommand;
    public DelegateCommand DeleteSaleLineCommand => _deleteSaleLineCommand ??= 
        new DelegateCommand(ExecuteDeleteSaleLineCommand, CanExecuteDeleteSaleLineCommand)
            .ObservesProperty(() => HasSelectedItems);

    private void ExecuteDeleteSaleLineCommand()
    {
        var itemsToRemove = SaleLines.Where(sl => sl.IsSelected).ToList();
        foreach (var item in itemsToRemove)
        {
            SaleLines.Remove(item);
        }
    }

    private bool CanExecuteDeleteSaleLineCommand() =>
        SaleLines.Any(sl => sl.IsSelected);
    
    private void CalculateSaleTotal()
    {
        AdjustmentTotalValue = SaleLines.Sum(s => s.Adjustment);
        AdjustmentTotal = AdjustmentTotalValue.ToString("C2");
        _variableCostsTotalValue = SaleLines.Sum(s => s.Costs);
        VariableCostsTotal = _variableCostsTotalValue.ToString("C2");
        ProfitTotalValue = SaleLines.Sum(s => s.Profit);
        ProfitTotal = ProfitTotalValue.ToString("C2");
        TotalValue = SaleLines.Sum(s => s.Subtotal);
        Total = TotalValue.ToString("C2");
    }
    
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
        
        if (_selectedDiscountSurchargeMode == DiscountSurcharge.Money)
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

    private void CleanUpLine()
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

    private void CleanUpTotals()
    {
        AdjustmentTotalValue = 0;
        AdjustmentTotal = 0.ToString("C2");
        _variableCostsTotalValue = 0;
        VariableCostsTotal = 0.ToString("C2");
        ProfitTotalValue = 0;
        ProfitTotal = 0.ToString("C2");
        TotalValue = 0;
        Total = 0.ToString("C2");
    }
    
    private void SaleLine_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(SaleLineViewModel.IsSelected)) return;
     
        bool allSelected = SaleLines.Any() && SaleLines.All(line => line.IsSelected);
        SetProperty(ref _isAllSaleLinesSelected, allSelected, nameof(IsAllSaleLinesSelected));
        
        bool anySelected = SaleLines.Any(line => line.IsSelected);
        SetProperty(ref _hasSelectedItems, anySelected, nameof(HasSelectedItems));
    }
    private void SaleLines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        bool allSelected = SaleLines.Any() && SaleLines.All(line => line.IsSelected);
        SetProperty(ref _isAllSaleLinesSelected, allSelected, nameof(IsAllSaleLinesSelected));
        CalculateSaleTotal();
        
        bool anySelected = SaleLines.Any(line => line.IsSelected);
        SetProperty(ref _hasSelectedItems, anySelected, nameof(HasSelectedItems));
    }
}