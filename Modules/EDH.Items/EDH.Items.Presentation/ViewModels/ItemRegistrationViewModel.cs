using System.Collections.ObjectModel;
using EDH.Core.Constants;
using EDH.Items.Application.DTOs;
using EDH.Items.Application.Services.Interfaces;
using EDH.Presentation.Common.UIModels;
using FluentValidation;
using EDH.Core.Extensions;

namespace EDH.Items.Presentation.ViewModels;

public class ItemRegistrationViewModel : BindableBase, INavigationAware
{
	private readonly IItemService _itemService;
	private readonly IRegionManager _regionManager;
	private readonly IDialogService _dialogService;
	private bool _isNavigationTarget = true;

	public ItemRegistrationViewModel(IItemService itemService, IRegionManager regionManager, IDialogService dialogService)
	{
		_itemService = itemService;
		_regionManager = regionManager;
		_dialogService = dialogService;
		VariableCosts.CollectionChanged += VariableCosts_CollectionChanged;
	}

	private string _name;
	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	private string _description;
	public string Description
	{
		get => _description;
		set => SetProperty(ref _description, value);
	}

	private List<string> _categories;
	public List<string> Categories
	{
		get => _categories;
		set => SetProperty(ref _categories, value);
	}

	public ObservableCollection<VariableCostModel> VariableCosts { get; } = [];


	private DelegateCommand? _addVariableCostCommand;
	public DelegateCommand AddVariableCostCommand => _addVariableCostCommand ??= new DelegateCommand(ExecuteAddVariableCostCommand);

	private void ExecuteAddVariableCostCommand()
	{
		VariableCosts.Add(new VariableCostModel());
	}

	private DelegateCommand<VariableCostModel>? _deleteVariableCostCommand;
	public DelegateCommand<VariableCostModel> DeleteVariableCostCommand => _deleteVariableCostCommand ??= new DelegateCommand<VariableCostModel>(ExecuteDeleteVariableCostCommand);

	private void ExecuteDeleteVariableCostCommand(VariableCostModel variableCost)
	{
		if (VariableCosts.Contains(variableCost))
		{
			VariableCosts.Remove(variableCost);
		}
	}

	private string _sellingPrice;
	public string SellingPrice
	{
		get => _sellingPrice;
		set => SetProperty(ref _sellingPrice, value);
	}

	private string _stockQuantity;

	public string StockQuantity
	{
		get => _stockQuantity;
		set => SetProperty(ref _stockQuantity, value);
	}

	private string _stockAlertThreshold;

	public string StockAlertThreshold
	{
		get => _stockAlertThreshold;
		set => SetProperty(ref _stockAlertThreshold, value);
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
			var itemDto = new ItemDto
			(
				Name: Name,
				Description: Description,
				SellingPrice: SellingPrice.ToDecimal(),
				Category: "test",
				VariableCosts: VariableCosts.Select(vc => new ItemVariableCostDto(vc.Name, vc.Value.ToDecimal()))
			);

			await _itemService.CreateItemAsync(itemDto);

			_dialogService.ShowDialog("OkDialog", new DialogParameters
			{
				{ "title", "Item Registration" },
				{ "message", $"Item '{Name}' has been registered successfully." }
			}); 

			Cleanup();
			_isNavigationTarget = false;
			_regionManager.RequestNavigate(NavigationConstants.Regions.MainWindowContent, NavigationConstants.Views.ItemRegistration);
		}
		catch (ValidationException ex)
		{
			_dialogService.ShowDialog("OkDialog", new DialogParameters
			{
				{ "title", "Item Registration" },
				{ "message", $"One or more errors occurred: {ex.Message}" }
			});
		}
		catch (Exception ex)
		{
			_dialogService.ShowDialog("OkDialog", new DialogParameters
			{
				{ "title", "Item Registration" },
				{ "message", $"Unknown error occurred: {ex.Message}" }
			});
		}
	}

	private bool CanExecuteRegisterNewItemCommand() =>
		(!String.IsNullOrWhiteSpace(Name) &&
		!String.IsNullOrWhiteSpace(SellingPrice) &&
		SellingPrice.TryToDecimal(out _)) &&
		(!VariableCosts.Any() ||
		VariableCosts.All(variableCost => variableCost.Value.TryToDecimal(out _))) &&
		(String.IsNullOrWhiteSpace(StockQuantity) || Int32.TryParse(StockQuantity, out _)) &&
		(String.IsNullOrWhiteSpace(StockAlertThreshold) || Int32.TryParse(StockAlertThreshold, out _));

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

	private void VariableCosts_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

		RegisterNewItemCommand?.RaiseCanExecuteChanged();
	}

	private void VariableCostItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(VariableCostModel.Value))
		{
			RegisterNewItemCommand?.RaiseCanExecuteChanged();
		}
	}

	public void Cleanup()
	{
		foreach (var item in VariableCosts)
		{
			item.PropertyChanged -= VariableCostItem_PropertyChanged;
		}
		VariableCosts.CollectionChanged -= VariableCosts_CollectionChanged;
	}
}