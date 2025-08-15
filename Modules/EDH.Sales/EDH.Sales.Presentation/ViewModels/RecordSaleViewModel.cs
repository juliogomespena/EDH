using System.Collections.Specialized;
using System.ComponentModel;
using EDH.Core.Constants;
using EDH.Core.Enums;
using EDH.Core.Extensions;
using EDH.Presentation.Common.Collections;
using EDH.Presentation.Common.ViewModels;
using EDH.Sales.Application.DTOs.Request.CreateSale;
using EDH.Sales.Application.DTOs.Request.Models;
using EDH.Sales.Application.DTOs.Request.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Request.SaleTotalCalculation;
using EDH.Sales.Application.DTOs.Response.GetInventoryItem;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Application.Validators.ItemDiscountSurcharge;
using EDH.Sales.Application.Validators.ItemQuantity;
using EDH.Sales.Application.Validators.ItemQuantity.Models;
using EDH.Sales.Presentation.UIModels;
using Microsoft.Extensions.Logging;

namespace EDH.Sales.Presentation.ViewModels;

internal sealed class RecordSaleViewModel : BaseViewModel, INavigationAware
{
    private readonly ISaleService _saleService;
    private readonly IDialogService _dialogService;
    private readonly IRegionManager _regionManager;
    private readonly ILogger<RecordSaleViewModel> _logger;
    private readonly ItemQuantityValidator _itemQuantityValidator = new ItemQuantityValidator();
    private readonly ItemDiscountSurchargeValidator _itemDiscountSurchargeValidator = new ItemDiscountSurchargeValidator();
    private bool _isNavigationTarget = true;
    private bool _isNavigatingInItemsComboBox;
    private bool _isSelectingItem;

    public RecordSaleViewModel(ISaleService saleService, IDialogService dialogService, IRegionManager regionManager, ILogger<RecordSaleViewModel> logger)
    {
        _saleService = saleService;
        _dialogService = dialogService;
        _regionManager = regionManager;
        _logger = logger;
        SelectedDiscountSurchargeMode = DiscountSurchargeMode.Money;
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
                { "message", "Unknown error searching for items." }
            });
            throw;
        }
    }

    private async Task SetComboBoxInventoryItems(string itemName)
    {
        var items = await _saleService.GetInventoryItemsByNameAsync(itemName);
        
        if (items.IsSuccess)
            Items = items.Value?.ToList() ?? [];
        
        if (SelectedItem is null) IsItemsDropdownOpen = true;
    }

    private GetInventoryItemResponse? _selectedItem;
    public GetInventoryItemResponse? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value)) return;
            _isSelectingItem = true;
            
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
            UnitPrice = value.ItemModel.Price.ToString("C2");
            _unitPriceValue = value.ItemModel.Price;
            _unitCostValue = value.ItemModel.VariableCost;
            CalculateLineSubTotals();

            _isSelectingItem = false;
            if (value.Quantity >= 1) return;
            
            SetError(nameof(ItemName), "There is no inventory availability for this item.");
        }
    }

    private List<GetInventoryItemResponse>? _items;
    public List<GetInventoryItemResponse> Items
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
            
            if (!_isSelectingItem)
                CalculateLineSubTotals();
        }
    }

    private void ValidateAndSetItemQuantity(string itemQuantity)
    {
        var input = 
            new ItemQuantityValidatorInputModel(itemQuantity, SelectedItem is not null, SelectedItem?.Quantity);
        
        var validationResult = _itemQuantityValidator.Validate(input);

        if (!validationResult.IsValid)
        {
            _itemQuantityValue = 0;
            string firstError = validationResult.Errors
                .FirstOrDefault()?.ErrorMessage
                ?? String.Empty;
            
            SetError(nameof(ItemQuantity), firstError);
            return;
        }
        
        _itemQuantityValue = Int32.TryParse(itemQuantity, out int itemQuantityParsed)
            ? itemQuantityParsed
            : 0;
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
            
            if (!_isSelectingItem)
                CalculateLineSubTotals();
        }
    }

    private void ValidateAndSetItemDiscountOrSurcharge(string itemDiscountOrSurcharge)
    {
        var validationResult = _itemDiscountSurchargeValidator.Validate(itemDiscountOrSurcharge);

        if (!validationResult.IsValid)
        {
            _itemDiscountOrSurchargeValue = 0;
            string firstError = validationResult.Errors
                .FirstOrDefault()?.ErrorMessage
                ?? String.Empty;
            
            SetError(nameof(ItemDiscountOrSurcharge), firstError);
            return;       
        }

        _itemDiscountOrSurchargeValue = itemDiscountOrSurcharge.TryToDecimal(out decimal parsedValue)
            ? parsedValue
            : 0;
        
        ClearError(nameof(ItemDiscountOrSurcharge));
    }

    public List<string> DiscountSurchargeModeRepresentation => ["$", "%"];
    
    private DiscountSurchargeMode _selectedDiscountSurchargeMode;
    public DiscountSurchargeMode SelectedDiscountSurchargeMode
    {
        get => _selectedDiscountSurchargeMode;
        set
        {
            if (!SetProperty(ref _selectedDiscountSurchargeMode, value)) return;

            if (!_isSelectingItem)
                CalculateLineSubTotals();
        }
    }

    private decimal _unitCostValue;
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
                SetError(nameof(ItemQuantity), $"Available in stock: {SelectedItem?.Quantity}.");
                return;           
            }
            
            sameItem.Quantity += _itemQuantityValue;
            sameItem.Costs += _variableCostsLineValue;
            sameItem.Adjustment += _selectedDiscountSurchargeMode == DiscountSurchargeMode.Money
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
            Adjustment = _selectedDiscountSurchargeMode == DiscountSurchargeMode.Money 
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
                .Select(sl => new SaleLineModel(
                    sl.ItemId, sl.ItemName, sl.UnitPrice, sl.Quantity, sl.Costs, sl.Adjustment, sl.Profit, sl.Subtotal, Currency.Usd
                ));
        
            var sale = new CreateSaleRequest(
                0, _variableCostsTotalValue, ProfitTotalValue, AdjustmentTotalValue, TotalValue, saleLines, Currency.Usd
            );
            
            var result = await _saleService.CreateSaleAsync(sale);

            if (result.IsFailure)
            {
                _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
                {
                    { "title", "Sale Registration" },
                    { "message", $"One or more errors occurred: {String.Join(' ', result.Errors)}." }
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
                { "message", "Unknown error occurred." }
            });
            throw;
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
        try
        {
            var request = new SaleTotalCalculationRequest(SaleLines.Select(sl => new SaleLineModel(sl.ItemId, sl.ItemName, sl.UnitPrice, sl.Quantity,
                sl.Costs, sl.Adjustment, sl.Profit, sl.Subtotal, Currency.Usd)).ToArray());

            var result = _saleService.CalculateSaleTotal(request);
                
            if (result.IsFailure)
            {
                _logger.LogError("Error calculating sale total: {Join}.", String.Join(' ', result.Errors));
                return;
            }
            
            var resultProperties = result.Value!;
            
            AdjustmentTotalValue = resultProperties.Adjustment;
            AdjustmentTotal = AdjustmentTotalValue.ToString("C2");
            _variableCostsTotalValue = resultProperties.Costs;
            VariableCostsTotal = _variableCostsTotalValue.ToString("C2");
            ProfitTotalValue = resultProperties.Profit;
            ProfitTotal = ProfitTotalValue.ToString("C2");
            TotalValue = resultProperties.Total;
            Total = TotalValue.ToString("C2");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, $"Error in {nameof(CalculateSaleTotal)}.");
            throw;
        }
    }
    
    private void CalculateLineSubTotals()
    {
        if (SelectedItem is null) return;
        
        try
        {
            var result = _saleService.CalculateSaleLine(new SaleLineCalculationRequest(_unitPriceValue,
                _itemQuantityValue,
                _unitCostValue, _itemDiscountOrSurchargeValue, _selectedDiscountSurchargeMode, Currency.Usd));

            if (result.IsFailure)
            {
                _logger.LogError("Error calculating line subtotal: {Join}.", String.Join(' ', result.Errors));
                return;
            }

            var resultProperties = result.Value!;

            _subTotalValue = resultProperties.Subtotal;
            SubTotal = _subTotalValue.ToString("C2");
            ProfitValue = resultProperties.Profit;
            Profit = ProfitValue.ToString("C2");
            _variableCostsLineValue = resultProperties.Costs;
            VariableCostsLineSum = _variableCostsLineValue.ToString("C2");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Error in {nameof(CalculateLineSubTotals)}.");
            throw;
        }
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