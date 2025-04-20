using System.Windows;

namespace EDH.Presentation.Common.ViewModels;

public class MainWindowMenuViewModel : BindableBase
{
	private Visibility _menuVisibility = Visibility.Collapsed;

	public Visibility MenuVisibility
	{
		get => _menuVisibility;
		set => SetProperty(ref _menuVisibility, value);
	}

	private Visibility _buttonCloseMenuVisibility = Visibility.Hidden;
	public Visibility ButtonCloseMenuVisibility
	{
		get => _buttonCloseMenuVisibility;
		set => SetProperty(ref _buttonCloseMenuVisibility, value);
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
		MenuVisibility = Visibility.Collapsed;
		ButtonCloseMenuVisibility = Visibility.Hidden;
		IsMenuItemsEnabled = false;
	}

	private DelegateCommand? _openMenuCommand;
	public DelegateCommand OpenMenuCommand => _openMenuCommand ??= new DelegateCommand(ExecuteOpenMenuCommand);

	private void ExecuteOpenMenuCommand()
	{
		MenuVisibility = Visibility.Visible;
		ButtonCloseMenuVisibility = Visibility.Visible;
		IsMenuItemsEnabled = true;
	}
}