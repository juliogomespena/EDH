using EDH.Core.Constants;
using EDH.Inventory.Application.DTOs.EditStockQuantities;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Presentation.Common.ViewModels;
using Microsoft.Extensions.Logging;

namespace EDH.Inventory.Presentation.Resources.Dialogs;

internal sealed class EditStockQuantitiesDialogViewModel : BaseViewModel, IDialogAware
{
	private readonly IInventoryItemService _inventoryItemService;
	private readonly IDialogService _dialogService;
	private readonly ILogger<EditStockQuantitiesDialogViewModel> _logger;
	private bool _isNavigatingInItemsComboBox;

	public EditStockQuantitiesDialogViewModel(IInventoryItemService inventoryItemService, IDialogService dialogService, ILogger<EditStockQuantitiesDialogViewModel> logger)
	{
		_inventoryItemService = inventoryItemService;
		_dialogService = dialogService;
		_logger = logger;
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

	private string? _itemName;
	public string ItemName
	{
		get => _itemName ?? String.Empty;
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
		var items = await _inventoryItemService.GetInventoryItemsByNameAsync(itemName);

		if (items.IsSuccess)
			Items = items.Value?.ToList() ?? [];
		
		if (SelectedItem is null) IsItemsDropdownOpen = true;
	}

	private GetInventoryItems? _selectedItem;
	public GetInventoryItems? SelectedItem
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
			CurrentStockQuantity = value.Quantity.ToString();
			StockAlertThreshold = value.AlertThreshold?.ToString() ?? String.Empty;
			_stockAlertThresholdValue = value.AlertThreshold;
			UpdatedStockQuantityValue = value.Quantity;
			UpdatedStockQuantity = value.Quantity.ToString();
		}
	}

	private List<GetInventoryItems>? _items;
	public List<GetInventoryItems> Items
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

	private string? _editStockQuantity;
	public string EditStockQuantity
	{
		get => _editStockQuantity ?? String.Empty;
		set {
			ValidateAndSetStockQuantity(value);
			
			SetProperty(ref _editStockQuantity, value);
		}
	}

	private void ValidateAndSetStockQuantity(string editStockQuantity)
	{
		if (String.IsNullOrWhiteSpace(editStockQuantity))
		{
			UpdatedStockQuantity = CurrentStockQuantity;
			ClearError(nameof(EditStockQuantity));
			return;
		}

		if (!Int32.TryParse(editStockQuantity, out int editStockQuantityParsed))
		{
			UpdatedStockQuantityValue = CurrentStockQuantityValue;
			SetError(nameof(EditStockQuantity), "Only whole numeric values allowed");
			return;
		}

		UpdatedStockQuantityValue = editStockQuantityParsed + CurrentStockQuantityValue;
		UpdatedStockQuantity = UpdatedStockQuantityValue?.ToString() ?? CurrentStockQuantity;

		if (UpdatedStockQuantityValue < 0)
		{
			SetError(nameof(EditStockQuantity), "Resulting quantity cannot be negative");
			return;
		}
		
		ClearError(nameof(EditStockQuantity));
	}

	private int? _stockAlertThresholdValue;
	private string? _stockAlertThreshold;
	public string StockAlertThreshold
	{
		get => _stockAlertThreshold ?? String.Empty;
		set
		{
			ValidateAndSetStockAlertThreshold(value);
			
			SetProperty(ref _stockAlertThreshold, value);
		}
	}

	private void ValidateAndSetStockAlertThreshold(string stockAlertThreshold)
	{
		if (String.IsNullOrWhiteSpace(stockAlertThreshold))
		{
			_stockAlertThresholdValue = null;
			ClearError(nameof(StockAlertThreshold));
			return;
		}

		if (!Int32.TryParse(stockAlertThreshold, out int stockAlertThresholdParsed) ||
		    stockAlertThresholdParsed < 0)
		{
			_stockAlertThresholdValue = -1;
			SetError(nameof(StockAlertThreshold), "Only whole numeric values greater or equal to 0 allowed");
			return;
		}
		
		_stockAlertThresholdValue = stockAlertThresholdParsed;
		ClearError(nameof(StockAlertThreshold));
	}

	private string? _currentStockQuantity;
	public string CurrentStockQuantity
	{
		get => _currentStockQuantity ?? String.Empty;
		set => SetProperty(ref _currentStockQuantity, value);
	}

	private int? _currentStockQuantityValue;
	public int? CurrentStockQuantityValue
	{
		get => _currentStockQuantityValue;
		set => SetProperty(ref _currentStockQuantityValue, value);
	}

	private string? _updatedStockQuantity;
	public string UpdatedStockQuantity
	{
		get => _updatedStockQuantity ?? String.Empty;
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
		UpdatedStockQuantityValue >= 0 &&
		(_stockAlertThresholdValue >= 0 || _stockAlertThresholdValue is null);

	private async void ExecuteSaveDialogCommand()
	{
		try
		{
			var updateStockQuantityDto = 
				new UpdateStockQuantities(SelectedItem!.Id, SelectedItem!.Name, (int)UpdatedStockQuantityValue!, _stockAlertThresholdValue);

			var result = await _inventoryItemService.UpdateStockQuantitiesAsync(updateStockQuantityDto);

			if (result.IsFailure)
			{
				_dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
				{
					{ "title", "Edit inventory" },
					{ "message", $"One or more errors occurred: {String.Join(' ', result.Errors)}" }
				});
				
				return;
			}

			_dialogService.ShowDialog(NavigationConstants.Dialogs.YesNoDialog, new DialogParameters
			{
				{ "title", "Edit inventory" },
				{ "message", $"Item {result.Value?.Id} '{result.Value?.ItemName}' has been updated successfully. Do you wish to update another item?" }
			}, dialogResult =>
			{
				if (dialogResult.Result is ButtonResult.No) RequestClose.Invoke();
			});

			CleanUp();
			ItemName = String.Empty;
			SelectedItem = null;
			Items = [];
			IsItemsDropdownOpen = false;
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(ExecuteSaveDialogCommand)}.");
			_dialogService.ShowDialog(NavigationConstants.Dialogs.OkDialog, new DialogParameters
			{
				{ "title", "Edit inventory" },
				{ "message", "Unknown error occurred" }
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