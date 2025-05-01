using EDH.Inventory.Application.DTOs;
using EDH.Inventory.Application.Services.Interfaces;

namespace EDH.Inventory.Presentation.Resources.Dialogs;

public class AddStockQuantitiesDialogViewModel : BindableBase, IDialogAware
{
	private readonly IInventoryItemService _inventoryItemService;
	private readonly IDialogService _dialogService;
	private bool _isNavigatingInItemsComboBox;

	public AddStockQuantitiesDialogViewModel(IInventoryItemService inventoryItemService, IDialogService dialogService)
	{
		_inventoryItemService = inventoryItemService;
		_dialogService = dialogService;
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
			_dialogService.ShowDialog("OkDialog", new DialogParameters
			{
				{ "title", "Inventory item search" },
				{ "message", $"Unknown error searching for items: {ex.Message}" }
			});
		}
	}

	private async Task SetComboBoxInventoryItems(string itemName)
	{
		var items = await _inventoryItemService.GetInventoryItemsByNameAsync(itemName);
		Items = items.ToList(); 
		if (SelectedItem is null) IsItemsDropdownOpen = true;
	}

	private GetInventoryItemDto? _selectedItem;
	public GetInventoryItemDto? SelectedItem
	{
		get => _selectedItem;
		set
		{
			if (!SetProperty(ref _selectedItem, value) || value is null) return;

			_isNavigatingInItemsComboBox = true;
			ItemName = value.Name;
			_isNavigatingInItemsComboBox = false;
		}
	}

	private List<GetInventoryItemDto> _items;
	public List<GetInventoryItemDto> Items
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

	private DelegateCommand? _saveDialogCommand;
	public DelegateCommand SaveDialogCommand => _saveDialogCommand ??= new DelegateCommand(ExecuteSaveDialogCommand);
	private void ExecuteSaveDialogCommand()
	{
		_dialogService.ShowDialog("OkDialog", new DialogParameters
		{
			{ "title", "Inventory item" },
			{ "message", $"{SelectedItem?.Name}" }
		});
	}

	private DelegateCommand? _cancelDialogCommand;
	public DelegateCommand CancelDialogCommand => _cancelDialogCommand ??= new DelegateCommand(ExecuteCancelDialogCommand);
	private void ExecuteCancelDialogCommand()
	{
		RequestClose.Invoke();
	}

	public bool CanCloseDialog()
	{
		return true;
	}

	public void OnDialogClosed()
	{
	}

	public void OnDialogOpened(IDialogParameters parameters)
	{
		
	}

	public DialogCloseListener RequestClose { get; }
}