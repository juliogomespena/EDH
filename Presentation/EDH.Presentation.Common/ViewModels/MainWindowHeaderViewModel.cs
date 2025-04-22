using EDH.Core.Events.UI;
using System.Windows;
using EDH.Core.Constants;

namespace EDH.Presentation.Common.ViewModels;

public sealed class MainWindowHeaderViewModel : BindableBase
{
	private readonly IRegionManager _regionManager;
	private readonly IEventAggregator _eventAggregator;
	private IRegionNavigationJournal? _journal;

	public MainWindowHeaderViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
	{
		_regionManager = regionManager;
		_eventAggregator = eventAggregator;
		var contentRegion = _regionManager.Regions[NavigationConstants.Regions.MainWindowContent];
		contentRegion.NavigationService.Navigated += ContentRegion_Navigated;
	}
	private void ContentRegion_Navigated(object? sender, RegionNavigationEventArgs e)
	{
		_journal = e.NavigationContext.NavigationService.Journal;
		GoBackCommand.RaiseCanExecuteChanged();
		GoForwardCommand.RaiseCanExecuteChanged();
	}

	private DelegateCommand? _openMenuTriggerCommand;
	public DelegateCommand OpenMenuTriggerCommand => _openMenuTriggerCommand ??= new DelegateCommand(ExecuteOpenMenuTriggerCommand);

	private void ExecuteOpenMenuTriggerCommand()
	{
		_eventAggregator.GetEvent<OpenMenuEvent>().Publish();
	}

	private DelegateCommand? _goBackCommand;
	public DelegateCommand GoBackCommand => _goBackCommand ??= new DelegateCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand);

	private void ExecuteGoBackCommand()
	{
		_journal?.GoBack();
	}

	private bool CanExecuteGoBackCommand()
	{
		return _journal is not null && _journal.CanGoBack;
	}

	private DelegateCommand? _goForwardCommand;
	public DelegateCommand GoForwardCommand => _goForwardCommand ??= new DelegateCommand(ExecuteGoForwardCommand, CanExecuteGoForwardCommand);

	private DelegateCommand? _goHomeCommand;
	public DelegateCommand GoHomeCommand => _goHomeCommand ??= new DelegateCommand(ExecuteGoHomeCommand);

	private void ExecuteGoHomeCommand()
	{
		//_regionManager.RequestNavigate(NavigationConstants.Regions.MainWindowContent, NavigationConstants.Views.MainWindowContent);
	}

	private void ExecuteGoForwardCommand()
	{
		_journal?.GoForward();
	}

	private bool CanExecuteGoForwardCommand()
	{
		return _journal is not null && _journal.CanGoForward;
	}

	private string? _exhibitionName = "Julio G. Pena";
	public string? ExhibitionName
	{
		get => _exhibitionName;
		set => SetProperty(ref _exhibitionName, value);
	}
}