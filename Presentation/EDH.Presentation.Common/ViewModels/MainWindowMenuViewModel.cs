using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using EDH.Core.Constants;
using EDH.Core.Events.UI;
using EDH.Presentation.Common.UIModels;

namespace EDH.Presentation.Common.ViewModels;

public sealed class MainWindowMenuViewModel : BindableBase
{
	private readonly IRegionManager _regionManager;
	private readonly IDialogService _dialogService;
	private bool _hasBeenOpened;

	public MainWindowMenuViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService)
	{
		_regionManager = regionManager;
		_dialogService = dialogService;
		eventAggregator.GetEvent<OpenMenuEvent>().Subscribe(OnOpenMenu);
		IsMenuOpen = false;
		IsMenuItemsEnabled = false;
		_hasBeenOpened = false;
		InitializeMenuItems();
	}

	private string _menuSearchText;

	public string MenuSearchText
	{
		get => _menuSearchText;
		set
		{
			SetProperty(ref _menuSearchText, value);
			FilterMenuItems(value);
		}
	}

	private bool _isMenuFiltered;
	private void FilterMenuItems(string filter)
	{
		if (_isMenuFiltered)
			MenuExhibitionItems = new ObservableCollection<MenuItemModel>(_menuItems);

		if (String.IsNullOrEmpty(filter))
		{
			_isMenuFiltered = false;
			return;
		}

		_isMenuFiltered = true;
		MenuExhibitionItems.Clear();

		foreach (var menu in _menuItems)
		{
			var subMenusToInclude = new ObservableCollection<SubMenuItemModel>(menu.SubItems.Where(subMenu =>
				subMenu.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)));

			if (!subMenusToInclude.Any()) continue;

			var menuItem = new MenuItemModel(menu.IconKind, menu.Header)
			{
				SubItems = subMenusToInclude,
				IsExpanded = true
			};
			MenuExhibitionItems.Add(menuItem);
		}
	}

	private ObservableCollection<MenuItemModel> _menuItems;
	private ObservableCollection<MenuItemModel> _menuExhibitionItems;
	public ObservableCollection<MenuItemModel> MenuExhibitionItems
	{
		get => _menuExhibitionItems;
		set => SetProperty(ref _menuExhibitionItems, value);
	}

	private bool _shouldAnimateClose;
	public bool ShouldAnimateClose
	{
		get => _shouldAnimateClose;
		set => SetProperty(ref _shouldAnimateClose, value);
	}

	private bool _isMenuOpen;
	public bool IsMenuOpen
	{
		get => _isMenuOpen;
		set => SetProperty(ref _isMenuOpen, value);
	}

	private bool _isMenuItemsEnabled;
	public bool IsMenuItemsEnabled
	{
		get => _isMenuItemsEnabled;
		set => SetProperty(ref _isMenuItemsEnabled, value);
	}

	private DelegateCommand? _closeMenuCommand;
	public DelegateCommand CloseMenuCommand => _closeMenuCommand ??= new DelegateCommand(ExecuteCloseMenuCommand);

	private void ExecuteCloseMenuCommand()
	{
		foreach (var menu in MenuExhibitionItems) menu.IsExpanded = false;

		if (_hasBeenOpened)
		{
			ShouldAnimateClose = true;
			Task.Delay(600).ContinueWith(_ =>
			{
				ShouldAnimateClose = false;
			});
		}

		IsMenuOpen = false;
		IsMenuItemsEnabled = false;

		Task.Delay(600).ContinueWith(_ =>
		{
			MenuSearchText = String.Empty;
		});
	}

	private DelegateCommand? _openMenuCommand;
	public DelegateCommand OpenMenuCommand => _openMenuCommand ??= new DelegateCommand(ExecuteOpenMenuCommand);

	private void ExecuteOpenMenuCommand()
	{
		_hasBeenOpened = true;
		IsMenuOpen = true;
		IsMenuItemsEnabled = true;
	}
	private void OnOpenMenu()
	{
		OpenMenuCommand.Execute();
	}

	private void InitializeMenuItems()
	{
		_menuItems = new ObservableCollection<MenuItemModel>();

		var itemsMenu = new MenuItemModel("Tag", "Items");
		itemsMenu.SubItems.Add(new SubMenuItemModel("Insert new", new DelegateCommand(OpenAddItemViewCommand)));
		itemsMenu.SubItems.Add(new SubMenuItemModel("Edit existing", new DelegateCommand(OpenEditItemCommand)));
		itemsMenu.SubItems.Add(new SubMenuItemModel("Show all", new DelegateCommand(NoMenuViewCommand)));
		itemsMenu.SubItems.Add(new SubMenuItemModel("Categories", new DelegateCommand(NoMenuViewCommand)));
		_menuItems.Add(itemsMenu);

		var inventoryMenu = new MenuItemModel("BoxVariant", "Inventory");
		inventoryMenu.SubItems.Add(new SubMenuItemModel("Detailed view", new DelegateCommand(NoMenuViewCommand)));
		inventoryMenu.SubItems.Add(new SubMenuItemModel("Edit item quantity", new DelegateCommand(OpenEditInventoryCommand)));
		inventoryMenu.SubItems.Add(new SubMenuItemModel("Movement history", new DelegateCommand(NoMenuViewCommand)));
		inventoryMenu.SubItems.Add(new SubMenuItemModel("Inventory report", new DelegateCommand(NoMenuViewCommand)));
		_menuItems.Add(inventoryMenu);

		MenuExhibitionItems = new ObservableCollection<MenuItemModel>(_menuItems);
	}

	private void OpenEditInventoryCommand()
	{
		_dialogService.Show("AddStockQuantitiesDialog");
	}

	private void OpenAddItemViewCommand()
	{
		_regionManager.RequestNavigate(NavigationConstants.Regions.MainWindowContent, NavigationConstants.Views.ItemRegistration);
	}

	private void OpenEditItemCommand()
	{
		_regionManager.RequestNavigate(NavigationConstants.Regions.MainWindowContent, NavigationConstants.Views.ItemEdit);
	}

	private void NoMenuViewCommand()
	{

	}
}