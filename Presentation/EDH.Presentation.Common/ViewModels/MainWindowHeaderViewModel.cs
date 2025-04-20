using EDH.Core.Events.UI;
using System.Windows;

namespace EDH.Presentation.Common.ViewModels;

public class MainWindowHeaderViewModel(IEventAggregator eventAggregator) : BindableBase
{
	private Visibility _buttonCloseMenuVisibility = Visibility.Hidden;
	public Visibility ButtonCloseMenuVisibility
	{
		get => _buttonCloseMenuVisibility;
		set => SetProperty(ref _buttonCloseMenuVisibility, value);
	}

	private Visibility _buttonOpenMenuVisibility = Visibility.Visible;
	public Visibility ButtonOpenMenuVisibility
	{
		get => _buttonOpenMenuVisibility;
		set => SetProperty(ref _buttonOpenMenuVisibility, value);
	}

	private DelegateCommand? _openMenuTriggerCommand;
	public DelegateCommand OpenMenuTriggerCommand => _openMenuTriggerCommand ??= new DelegateCommand(ExecuteOpenMenuTriggerCommand);

	private void ExecuteOpenMenuTriggerCommand()
	{
		ButtonCloseMenuVisibility = Visibility.Visible;
		eventAggregator.GetEvent<OpenMenuEvent>().Publish();
	}

	private DelegateCommand? _closeMenuTriggerCommand;
	public DelegateCommand CloseMenuTriggerCommand => _closeMenuTriggerCommand ??= new DelegateCommand(ExecuteCloseMenuTriggerCommand);

	private void ExecuteCloseMenuTriggerCommand()
	{
		ButtonCloseMenuVisibility = Visibility.Hidden;
		eventAggregator.GetEvent<CloseMenuEvent>().Publish();
	}

	private string? _exhibitionName = "Julio G. Pena";
	public string? ExhibitionName
	{
		get => _exhibitionName;
		set => SetProperty(ref _exhibitionName, value);
	}	
}