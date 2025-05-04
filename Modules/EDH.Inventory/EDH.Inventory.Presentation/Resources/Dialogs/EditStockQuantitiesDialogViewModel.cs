using EDH.Core.Constants;
using EDH.Core.Exceptions;
using EDH.Inventory.Application.DTOs.EditStockQuantities;
using EDH.Inventory.Application.Services.Interfaces;
using FluentValidation;

namespace EDH.Inventory.Presentation.Resources.Dialogs;

internal sealed class EditStockQuantitiesDialogViewModel : BindableBase, IDialogAware
{
	private readonly IInventoryItemService _inventoryItemService;
	private readonly IDialogService _dialogService;
	private bool _isNavigatingInItemsComboBox;

	public EditStockQuantitiesDialogViewModel(IInventoryItemService inventoryItemService, IDialogService dialogService)
	{
		_inventoryItemService = inventoryItemService;
		_dialogService = dialogService;
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
		var items = await _inventoryItemService.GetInventoryItemsByNameAsync(itemName);
		Items = items.ToList(); 
		if (SelectedItem is null) IsItemsDropdownOpen = true;
	}

	private GetInventoryItemsEditStockQuantitiesDto? _selectedItem;
	public GetInventoryItemsEditStockQuantitiesDto? SelectedItem
	{
		get => _selectedItem;
		set
		{
			CleanUp();

			if (!SetProperty(ref _selectedItem, value) || value is null) return;

			_isNavigatingInItemsComboBox = true;
			ItemName = value.Name;
			_isNavigatingInItemsComboBox = false;

			CurrentStockQuantityValue = value.Quantity;
			CurrentStockQuantity = value.Quantity.ToString() ?? String.Empty;
			StockAlertThreshold = value.AlertThreshold?.ToString() ?? String.Empty;
			_stockAlertThresholdValue = value.AlertThreshold;
			UpdatedStockQuantityValue = value.Quantity;
			UpdatedStockQuantity = value.Quantity.ToString() ?? String.Empty;
		}
	}

	private List<GetInventoryItemsEditStockQuantitiesDto> _items;
	public List<GetInventoryItemsEditStockQuantitiesDto> Items
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

	private string _editStockQuantity;
	public string EditStockQuantity
	{
		get => _editStockQuantity;
		set {
			if (!SetProperty(ref _editStockQuantity, value)) return;

			if (String.IsNullOrWhiteSpace(value))
			{
				UpdatedStockQuantity = CurrentStockQuantity;
				return;
			}

			UpdatedStockQuantityValue = Int32.TryParse(value, out int editStockQuantity) && Int32.TryParse(CurrentStockQuantity, out int currentStockQuantity) ? (editStockQuantity + currentStockQuantity) : CurrentStockQuantityValue;
			UpdatedStockQuantity = UpdatedStockQuantityValue?.ToString() ?? CurrentStockQuantity;
		}
	}

	private int? _stockAlertThresholdValue;
	private string _stockAlertThreshold;
	public string StockAlertThreshold
	{
		get => _stockAlertThreshold;
		set
		{
			if (!SetProperty(ref _stockAlertThreshold, value)) return;

			if (String.IsNullOrWhiteSpace(value))
			{
				_stockAlertThresholdValue = null;
				return;
			}

			_stockAlertThresholdValue = Int32.TryParse(value, out int stockAlertThreshold) ? stockAlertThreshold : null;
		}
	}

	private string _currentStockQuantity;
	public string CurrentStockQuantity
	{
		get => _currentStockQuantity;
		set => SetProperty(ref _currentStockQuantity, value);
	}

	private int? _currentStockQuantityValue;
	public int? CurrentStockQuantityValue
	{
		get => _currentStockQuantityValue;
		set => SetProperty(ref _currentStockQuantityValue, value);
	}

	private string _updatedStockQuantity;
	public string UpdatedStockQuantity
	{
		get => _updatedStockQuantity;
		set => SetProperty(ref _updatedStockQuantity, value);
	}

	private int? _updatedStockQuantityValue;
	public int? UpdatedStockQuantityValue
	{
		get => _updatedStockQuantityValue;
		set => SetProperty(ref _updatedStockQuantityValue, value);
	}

	private DelegateCommand? _saveDialogCommand;
	public DelegateCommand SaveDialogCommand => 
		_saveDialogCommand ??= new DelegateCommand(ExecuteSaveDialogCommand, CanExecuteSaveDialogCommand)
			.ObservesProperty(() => SelectedItem)
			.ObservesProperty(() => EditStockQuantity)
			.ObservesProperty(() => StockAlertThreshold);

	private bool CanExecuteSaveDialogCommand() =>
		SelectedItem is not null &&
		(String.IsNullOrWhiteSpace(EditStockQuantity) || Int32.TryParse(EditStockQuantity, out _)) &&
		(String.IsNullOrWhiteSpace(StockAlertThreshold) || Int32.TryParse(StockAlertThreshold, out _)) &&
		UpdatedStockQuantityValue >= 0;

	private async void ExecuteSaveDialogCommand()
	{
		try
		{
			var updateStockQuantityDto = 
				new UpdateStockQuantitiesDto(SelectedItem!.Id, SelectedItem!.Name, (int)UpdatedStockQuantityValue!, _stockAlertThresholdValue);

			await _inventoryItemService.UpdateStockQuantitiesAsync(updateStockQuantityDto);

			_dialogService.ShowDialog(NavigationConstants.Dialogs.YesNoDialog, new DialogParameters
			{
				{ "title", "Edit inventory" },
				{ "message", $"Item '{ItemName}' has been updated successfully. Do you wish to update another item?" }
			}, result =>
			{
				if (result.Result is ButtonResult.No) RequestClose.Invoke();
			});

			CleanUp();
			ItemName = String.Empty;
			SelectedItem = null;
			Items = [];
			IsItemsDropdownOpen = false;
		}
		catch (ValidationException ex)
		{
			_dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
			{
				{ "title", "Edit inventory" },
				{ "message", $"One or more errors occurred: {ex.Message}" }
			});
		}
		catch (NotFoundException ex)
		{
			_dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
			{
				{ "title", "Edit inventory" },
				{ "message", ex.Message }
			});
		}
		catch (Exception ex)
		{
			_dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
			{
				{ "title", "Edit inventory" },
				{ "message", $"Unknown error occurred: {ex.Message}" }
			});
		}
	}

	private DelegateCommand? _cancelDialogCommand;
	public DelegateCommand CancelDialogCommand => _cancelDialogCommand ??= new DelegateCommand(ExecuteCancelDialogCommand);

	private void ExecuteCancelDialogCommand()
	{
		RequestClose.Invoke(new DialogResult(ButtonResult.No));
	}

	private void CleanUp()
	{
		CurrentStockQuantity = String.Empty;
		CurrentStockQuantityValue = null;
		UpdatedStockQuantity = String.Empty;
		UpdatedStockQuantityValue = null; 
		EditStockQuantity = String.Empty;
		StockAlertThreshold = String.Empty;
	}
}