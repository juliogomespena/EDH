using EDH.Presentation.Common.ViewModels;

namespace EDH.Presentation.Common.Resources.Dialogs;

internal sealed class OkDialogViewModel : BaseViewModel, IDialogAware
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

	private DelegateCommand? _okDialogCommand;
	public DelegateCommand OkDialogCommand => _okDialogCommand ??= new DelegateCommand(ExecuteOkDialogCommand);

	private void ExecuteOkDialogCommand()
	{
		RequestClose.Invoke(new DialogResult(ButtonResult.OK));
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