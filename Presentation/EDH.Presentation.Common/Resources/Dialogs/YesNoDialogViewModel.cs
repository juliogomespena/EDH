using EDH.Presentation.Common.ViewModels;

namespace EDH.Presentation.Common.Resources.Dialogs;

internal sealed class YesNoDialogViewModel : BaseViewModel, IDialogAware
{
	private string? _title;
	public string? Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}

	private string? _message;
	public string? Message
	{
		get => _message;
		set => SetProperty(ref _message, value);
	}

	private DelegateCommand? _yesDialogCommand;
	public DelegateCommand YesDialogCommand => _yesDialogCommand ??= new DelegateCommand(ExecuteYesDialogCommand);
	private void ExecuteYesDialogCommand()
	{
		RequestClose.Invoke(new DialogResult(ButtonResult.Yes));
	}

	private DelegateCommand? _noDialogCommand;
	public DelegateCommand NoDialogCommand => _noDialogCommand ??= new DelegateCommand(ExecuteNoDialogCommand);

	private void ExecuteNoDialogCommand()
	{
		RequestClose.Invoke(new DialogResult(ButtonResult.No));
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
		Title = parameters.GetValue<string>("title");
		Message = parameters.GetValue<string>("message");
	}

	public DialogCloseListener RequestClose { get; }
}