using System.Collections.ObjectModel;
using EDH.Core.Constants;
using EDH.Items.Application.Services.Interfaces;
using FluentValidation;
using EDH.Core.Extensions;
using EDH.Items.Application.DTOs.CreateItem;
using EDH.Items.Presentation.UIModels;
using EDH.Presentation.Common.ViewModels;
using Microsoft.Extensions.Logging;

namespace EDH.Items.Presentation.ViewModels;

internal sealed class ItemRegistrationViewModel : BaseViewModel, INavigationAware
{
    private readonly IItemService _itemService;
    private readonly IItemCategoryService _itemCategoryService;
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private readonly ILogger<ItemRegistrationViewModel> _logger;
    private bool _isNavigationTarget = true;
    private bool _isNavigatingInCategoriesComboBox;

    public ItemRegistrationViewModel(IItemService itemService, IItemCategoryService itemCategoryService,
        IRegionManager regionManager, IDialogService dialogService, ILogger<ItemRegistrationViewModel> logger)
    {
        _itemService = itemService;
        _itemCategoryService = itemCategoryService;
        _regionManager = regionManager;
        _dialogService = dialogService;
        _logger = logger;
        VariableCosts.CollectionChanged += VariableCosts_CollectionChanged;
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        try
        {
            await LoadItemsCategoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to load item registration view model.");
            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Navigation Error" },
                { "message", "Failed to load item registration" }
            });
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return _isNavigationTarget;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    private string? _name;
    public string Name
    {
        get => _name ?? String.Empty;
        set
        {
            ValidateName(value);

            SetProperty(ref _name, value);
        }
    }

    private void ValidateName(string name)
    {
        if (String.IsNullOrWhiteSpace(name))
        {
            SetError(nameof(Name), "Item name is required");
            return;
        }
        
        ClearError(nameof(Name));
    }

    private string? _description;
    public string Description
    {
        get => _description ?? String.Empty;
        set => SetProperty(ref _description, value);
    }

    private List<CreateItemCategoryDto> _categoriesPool = [];
    private List<CreateItemCategoryDto> _categories = [];
    public List<CreateItemCategoryDto> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    private CreateItemCategoryDto? _selectedItemCategory;
    public CreateItemCategoryDto? SelectedItemCategory
    {
        get => _selectedItemCategory;
        set
        {
            if (!SetProperty(ref _selectedItemCategory, value) || value is null) return;

            _isNavigatingInCategoriesComboBox = true;
            CategoryText = value.Name;
            _isNavigatingInCategoriesComboBox = false;
        }
    }

    private string _categoryText = String.Empty;
    public string CategoryText
    {
        get => _categoryText;
        set
        {
            if (!SetProperty(ref _categoryText, value)) return;

            if (_isNavigatingInCategoriesComboBox) return;

            if (String.IsNullOrWhiteSpace(value))
            {
                Categories = _categoriesPool;
                SelectedItemCategory = null;
                return;
            }

            Categories = _categoriesPool.Where(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var matchingCategory = Categories.FirstOrDefault(c =>
                c.Name.Equals(value, StringComparison.OrdinalIgnoreCase));

            SelectedItemCategory = matchingCategory ?? new CreateItemCategoryDto(0, value, null);

            if (SelectedItemCategory.Id == 0 || SelectedItemCategory is null) IsCategoryDropdownOpen = true;
        }
    }

    private bool _isCategoryDropdownOpen;
    public bool IsCategoryDropdownOpen
    {
        get => _isCategoryDropdownOpen;
        set => SetProperty(ref _isCategoryDropdownOpen, value);
    }

    public ObservableCollection<VariableCostModel> VariableCosts { get; } = [];

    private DelegateCommand? _addVariableCostCommand;
    public DelegateCommand AddVariableCostCommand =>
        _addVariableCostCommand ??= new DelegateCommand(ExecuteAddVariableCostCommand);

    private void ExecuteAddVariableCostCommand()
    {
        VariableCosts.Add(new VariableCostModel());
    }

    private DelegateCommand<VariableCostModel>? _deleteVariableCostCommand;
    public DelegateCommand<VariableCostModel> DeleteVariableCostCommand => _deleteVariableCostCommand ??=
        new DelegateCommand<VariableCostModel>(ExecuteDeleteVariableCostCommand);

    private void ExecuteDeleteVariableCostCommand(VariableCostModel variableCost)
    {
        if (VariableCosts.Contains(variableCost))
        {
            VariableCosts.Remove(variableCost);
        }
    }

    private string? _sellingPrice;
    public string SellingPrice
    {
        get => _sellingPrice ?? String.Empty;
        set
        {
            ValidateSellingPrice(value);
            
            if (!SetProperty(ref _sellingPrice, value)) return;
            
            CalculateProfitMargins();
        }
    }

    private void ValidateSellingPrice(string sellingPrice)
    {
        if (String.IsNullOrWhiteSpace(sellingPrice))
        {
            SetError(nameof(SellingPrice), "Selling price is required");
            return;
        }

        if (!sellingPrice.TryToDecimal(out decimal sellingPriceParsed) ||
            sellingPriceParsed <= 0)
        {
            SetError(nameof(SellingPrice), "Only positive numeric values allowed");
            return;
        }
		
        ClearError(nameof(SellingPrice));
    }

    private string? _stockQuantity;
    public string StockQuantity
    {
        get => _stockQuantity ?? String.Empty;
        set
        {
            ValidateStockQuantity(value);
            
            SetProperty(ref _stockQuantity, value);
        }
    }

    private void ValidateStockQuantity(string stockQuantity)
    {
        if (String.IsNullOrWhiteSpace(stockQuantity))
        {
            ClearError(nameof(StockQuantity));
            return;
        }
        
        if (!Int32.TryParse(stockQuantity, out int stockQuantityParsed) ||
            stockQuantityParsed <= 0)
        {
            SetError(nameof(StockQuantity), "Only whole positive numeric values allowed");
            return;
        }
		
        ClearError(nameof(StockQuantity));
    }

    private string? _stockAlertThreshold;
    public string StockAlertThreshold
    {
        get => _stockAlertThreshold ?? String.Empty;
        set
        {
            ValidateStockAlertThreshold(value);
            
            SetProperty(ref _stockAlertThreshold, value);
        }
    }

    private void ValidateStockAlertThreshold(string stockAlertThreshold)
    {
        if (String.IsNullOrWhiteSpace(stockAlertThreshold))
        {
            ClearError(nameof(StockAlertThreshold));
            return;
        }
        
        if (!Int32.TryParse(stockAlertThreshold, out int stockAlertThresholdParsed) ||
            stockAlertThresholdParsed <= 0)
        {
            SetError(nameof(StockAlertThreshold), "Only whole positive numeric values allowed");
            return;
        }
		
        ClearError(nameof(StockAlertThreshold));
    }

    private string _profitMargin = 0.ToString("C2");
    public string ProfitMargin
    {
        get => _profitMargin;
        set => SetProperty(ref _profitMargin, value);
    }

    private string _marginPercentage = 0.ToString("P2");
    public string MarginPercentage
    {
        get => _marginPercentage;
        set => SetProperty(ref _marginPercentage, value);
    }

    private decimal _profitValue;
    public decimal ProfitValue
    {
        get => _profitValue;
        private set => SetProperty(ref _profitValue, value);
    }

    private decimal _marginPercentageValue;
    public decimal MarginPercentageValue
    {
        get => _marginPercentageValue;
        private set => SetProperty(ref _marginPercentageValue, value);
    }

    private DelegateCommand? _registerNewItemCommand;
    public DelegateCommand RegisterNewItemCommand =>
        _registerNewItemCommand ??=
            new DelegateCommand(ExecuteRegisterNewItemCommand, CanExecuteRegisterNewItemCommand)
                .ObservesProperty(() => Name)
                .ObservesProperty(() => Description)
                .ObservesProperty(() => Categories)
                .ObservesProperty(() => VariableCosts)
                .ObservesProperty(() => SellingPrice)
                .ObservesProperty(() => StockQuantity)
                .ObservesProperty(() => StockAlertThreshold);

    private async void ExecuteRegisterNewItemCommand()
    {
        try
        {
            bool shouldProceed = true;

            if (SelectedItemCategory is not null && SelectedItemCategory.Id == 0)
            {
                _dialogService.ShowDialog(NavigationConstants.Dialogs.YesNoDialog, new DialogParameters
                {
                    { "title", "Item Category" },
                    {
                        "message",
                        $"The item category '{SelectedItemCategory.Name}' does not exist. Click YES to create it along with the item. Otherwise, click NO and erase or correct the category."
                    }
                }, dialogResult =>
                {
                    if (dialogResult.Result is ButtonResult.No) shouldProceed = false;
                });
            }

            if (!shouldProceed) return;

            var itemDto = new CreateItemDto
            (
                Id: 0,
                Name: Name,
                Description: Description,
                SellingPrice: SellingPrice.ToDecimal(),
                ItemCategory: SelectedItemCategory,
                VariableCosts: VariableCosts.Select(vc => new CreateItemVariableCostDto
                (
                    vc.Name,
                    vc.Value.ToDecimal()
                )),
                Inventory: !String.IsNullOrWhiteSpace(StockQuantity) || !String.IsNullOrWhiteSpace(StockAlertThreshold)
                    ? new CreateItemInventoryDto
                    (
                        InitialStock: Int32.TryParse(StockQuantity, out int initialStock) ? initialStock : null,
                        StockAlertThreshold: Int32.TryParse(StockAlertThreshold, out int alertThreshold)
                            ? alertThreshold
                            : null
                    )
                    : null
            );

            var result = await _itemService.CreateItemAsync(itemDto);

            if (result.IsFailure)
            {
                _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
                {
                    { "title", "Item Registration" },
                    { "message", $"One or more errors occurred: {String.Join(' ', result.Errors)}" }
                });
                
                return;
            }

            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Item Registration" },
                { "message", $"Item {result.Value.Id} '{result.Value.Name}' has been registered successfully." }
            });

            Cleanup();
            _isNavigationTarget = false;
            _regionManager.RequestNavigate(NavigationConstants.Regions.MainWindowContent,
                NavigationConstants.Views.ItemRegistration);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to register new item.");
            _dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
            {
                { "title", "Item Registration" },
                { "message", "Unknown error occurred" }
            });
        }
    }

    private bool CanExecuteRegisterNewItemCommand() =>
        (!String.IsNullOrWhiteSpace(Name) &&
         !String.IsNullOrWhiteSpace(SellingPrice) &&
         SellingPrice.TryToDecimal(out _)) &&
        (!VariableCosts.Any() ||
         VariableCosts.All(variableCost => variableCost.Value.TryToDecimal(out _) && !String.IsNullOrWhiteSpace(variableCost.Name))) &&
        (String.IsNullOrWhiteSpace(StockQuantity) || Int32.TryParse(StockQuantity, out _)) &&
        (String.IsNullOrWhiteSpace(StockAlertThreshold) || Int32.TryParse(StockAlertThreshold, out _));

    private void VariableCosts_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (VariableCostModel item in e.NewItems)
            {
                item.PropertyChanged += VariableCostItem_PropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (VariableCostModel item in e.OldItems)
            {
                item.PropertyChanged -= VariableCostItem_PropertyChanged;
            }
        }

        RegisterNewItemCommand.RaiseCanExecuteChanged();
        CalculateProfitMargins();
    }

    private void VariableCostItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VariableCostModel.Value))
        {
            RegisterNewItemCommand.RaiseCanExecuteChanged();
            CalculateProfitMargins();
            return;
        }
        if (e.PropertyName == nameof(VariableCostModel.Name))
        {
            RegisterNewItemCommand.RaiseCanExecuteChanged();
            CalculateProfitMargins();
        }
    }

    private void Cleanup()
    {
        foreach (var item in VariableCosts)
        {
            item.PropertyChanged -= VariableCostItem_PropertyChanged;
        }

        VariableCosts.CollectionChanged -= VariableCosts_CollectionChanged;
    }

    private void CalculateProfitMargins()
    {
        if (!SellingPrice.TryToDecimal(out decimal sellingPriceValue))
        {
            ProfitMargin = 0.ToString("C2");
            MarginPercentage = 0.ToString("P2");
            ProfitValue = 0;
            MarginPercentageValue = 0;
            return;
        }

        decimal totalCost = VariableCosts
            .Where(vc => vc.Value.TryToDecimal(out _))
            .Sum(vc => vc.Value.ToDecimal());

        decimal profit = sellingPriceValue - totalCost;
        ProfitValue = profit;
        MarginPercentageValue = sellingPriceValue == 0
            ? 0
            : profit / sellingPriceValue;

        ProfitMargin = profit.ToString("C2");
        MarginPercentage = MarginPercentageValue.ToString("P2");
    }

    private async Task LoadItemsCategoriesAsync()
    {
        var categoriesResult = await _itemCategoryService.GetAllItemCategoriesAsync();
        if (categoriesResult.IsSuccess)
        {
            _categoriesPool = categoriesResult.Value.ToList();
            Categories = _categoriesPool;
        }
    }
}