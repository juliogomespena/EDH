using EDH.Core.Events.UI;
using System.Windows;

namespace EDH.Presentation.Common.ViewModels;

public class MainWindowHeaderViewModel(IEventAggregator eventAggregator) : BindableBase
{
	private DelegateCommand? _openMenuTriggerCommand;
	public DelegateCommand OpenMenuTriggerCommand => _openMenuTriggerCommand ??= new DelegateCommand(ExecuteOpenMenuTriggerCommand);

	private void ExecuteOpenMenuTriggerCommand()
	{
		eventAggregator.GetEvent<OpenMenuEvent>().Publish();
	}

	private string? _exhibitionName = "Julio G. Pena";
	public string? ExhibitionName
	{
		get => _exhibitionName;
		set => SetProperty(ref _exhibitionName, value);
	}	
}