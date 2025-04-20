using System.Windows;
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
}