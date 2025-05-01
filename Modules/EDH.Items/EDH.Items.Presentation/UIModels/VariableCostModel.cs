namespace EDH.Items.Presentation.UIModels;

internal sealed class VariableCostModel : BindableBase
{
	private string _name;
	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	private string _value;
	public string Value
	{
		get => _value;
		set => SetProperty(ref _value, value);
	}
}