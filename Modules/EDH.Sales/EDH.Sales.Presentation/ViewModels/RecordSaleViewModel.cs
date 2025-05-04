using EDH.Core.Constants;
using EDH.Sales.Application.DTOs;
using EDH.Sales.Application.Services.Interfaces;

namespace EDH.Sales.Presentation.ViewModels;

internal sealed class RecordSaleViewModel : BindableBase, INavigationAware
{
    private readonly ISalesService _salesService;
    private readonly IDialogService _dialogService;
    private bool _isNavigationTarget = true;
    private bool _isNavigatingInItemsComboBox;

    public RecordSaleViewModel(ISalesService salesService, IDialogService dialogService)
    {
        _salesService = salesService;
        _dialogService = dialogService;
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

    private string _itemName;

    public string ItemName
    {
        get => _itemName;
        set
        {
            if (!SetProperty(ref _itemName, value)) return;

            if (_isNavigatingInItemsComboBox) return;

            SelectedItem = null;
            CleanUp();

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
            await SetComboBoxInventoryItems(_itemName);
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
            CleanUp();

            if (!SetProperty(ref _selectedItem, value) || value is null) return;

            _isNavigatingInItemsComboBox = true;
            ItemName = value.Name;
            _isNavigatingInItemsComboBox = false;
        }
    }

    private List<GetInventoryItemsRecordSaleDto> _items;

    public List<GetInventoryItemsRecordSaleDto> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    private bool _isItemsDropdownOpen;

    public bool IsItemsDropdownOpen
    {
        get => _isItemsDropdownOpen;
        set => SetProperty(ref _isItemsDropdownOpen, value);
    }

    private int? _itemQuantityValue;
    private string _itemQuantity;

    public string ItemQuantity
    {
        get => _itemQuantity;
        set
        {
            if (!SetProperty(ref _itemQuantity, value)) return;

            if (String.IsNullOrWhiteSpace(value))
            {
                _itemQuantityValue = null;
                return;
            }
            
            _itemQuantityValue = Int32.TryParse(value, out int itemQuantity) ? itemQuantity : null;
        }
    }

    private void CleanUp()
    {
    }
}