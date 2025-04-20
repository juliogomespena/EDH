using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EDH.Core.Events.UI;

namespace EDH.Presentation.Common.ViewModels;

public class MainWindowMenuViewModel : BindableBase
{
	private bool _hasBeenOpened;

	public MainWindowMenuViewModel(IEventAggregator eventAggregator)
	{
		eventAggregator.GetEvent<OpenMenuEvent>().Subscribe(OnOpenMenu);
		eventAggregator.GetEvent<CloseMenuEvent>().Subscribe(OnCloseMenu);
		IsMenuOpen = false;
		IsMenuItemsEnabled = false;
		_hasBeenOpened = false;
		InitializeMenuItems();
	}

	private ObservableCollection<MenuItem> _menuItems;

	public ObservableCollection<MenuItem> MenuItems
	{
		get => _menuItems;
		set => SetProperty(ref _menuItems, value);
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
		foreach (var menu in MenuItems) menu.IsExpanded = false;

		if (_hasBeenOpened)
		{
			ShouldAnimateClose = true;
			System.Threading.Tasks.Task.Delay(600).ContinueWith(_ =>
			{
				ShouldAnimateClose = false;
			});
		}

		IsMenuOpen = false;
		IsMenuItemsEnabled = false;
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

	private void OnCloseMenu()
	{
		CloseMenuCommand.Execute();
	}

	private void InitializeMenuItems()
	{
		MenuItems = new ObservableCollection<MenuItem>();

		var itemsMenu = new MenuItem("Tag", "Items");
		itemsMenu.SubItems.Add(new SubMenuItem("Insert new", new DelegateCommand(OpenAddItemViewCommand)));
		itemsMenu.SubItems.Add(new SubMenuItem("Edit existing", new DelegateCommand(OpenAddItemViewCommand)));
		itemsMenu.SubItems.Add(new SubMenuItem("Show all", new DelegateCommand(OpenAddItemViewCommand)));
		itemsMenu.SubItems.Add(new SubMenuItem("Categories", new DelegateCommand(OpenAddItemViewCommand)));
		MenuItems.Add(itemsMenu);

		var inventoryMenu = new MenuItem("BoxVariant", "Inventory");
		inventoryMenu.SubItems.Add(new SubMenuItem("Detailed view", new DelegateCommand(OpenAddItemViewCommand)));
		inventoryMenu.SubItems.Add(new SubMenuItem("Edit item quantity", new DelegateCommand(OpenAddItemViewCommand)));
		inventoryMenu.SubItems.Add(new SubMenuItem("Movement history", new DelegateCommand(OpenAddItemViewCommand)));
		inventoryMenu.SubItems.Add(new SubMenuItem("Inventory report", new DelegateCommand(OpenAddItemViewCommand)));
		MenuItems.Add(inventoryMenu);
	}

	private void OpenAddItemViewCommand()
	{
		
	}
}

public class MenuItem : BindableBase
{
	public MenuItem(string iconKind, string header)
	{
		IconKind = iconKind;
		Header = header;
		IsExpanded = false;
		SubItems = new ObservableCollection<SubMenuItem>();
	}

	private string _iconKind = null!;
	public string IconKind
	{
		get => _iconKind;
		set => SetProperty(ref _iconKind, value);
	}

	private string _header = null!;
	public string Header
	{
		get => _header;
		set => SetProperty(ref _header, value);
	}

	private bool _isExpanded;
	public bool IsExpanded
	{
		get => _isExpanded;
		set => SetProperty(ref _isExpanded, value);
	}

	private ObservableCollection<SubMenuItem> _subItems = null!;
	public ObservableCollection<SubMenuItem> SubItems
	{
		get => _subItems;
		set => SetProperty(ref _subItems, value);
	}
}

public class SubMenuItem : BindableBase
{
	public SubMenuItem(string name, ICommand command)
	{
		Name = name;
		Command = command;
	}

	private string _name = null!;
	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	private ICommand _command = null!;
	public ICommand Command
	{
		get => _command;
		set => SetProperty(ref _command, value);
	}
}