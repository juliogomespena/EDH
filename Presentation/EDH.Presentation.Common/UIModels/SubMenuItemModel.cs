using System.Windows.Input;

namespace EDH.Presentation.Common.UIModels;

internal sealed class SubMenuItemModel : BindableBase
{
	public SubMenuItemModel(string name, ICommand command)
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