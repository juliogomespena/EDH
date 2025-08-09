using EDH.Core.Extensions;
using EDH.Presentation.Common.ViewModels;

namespace EDH.Items.Presentation.UIModels;

internal sealed class VariableCostModel : BaseViewModel
{
	private string? _name;
	public string Name
	{
		get => _name ?? String.Empty;
		set
		{
			ValidateName(value);

			SetProperty(ref _name, value);
		}
	}

	private void ValidateName(string name)
	{
		if (String.IsNullOrWhiteSpace(name))
		{
			SetError(nameof(Name), "Cost name is required");
			return;
		}
		
		ClearError(nameof(Name));
	}

	private string? _value;
	public string Value
	{
		get => _value ?? String.Empty;
		set
		{
			ValidateValue(value);

			SetProperty(ref _value, value);
		}
	}

	private void ValidateValue(string value)
	{
		if (String.IsNullOrWhiteSpace(value))
		{
			SetError(nameof(Value), "Cost value is required");
			return;
		}

		if (!value.TryToDecimal(out decimal valueParsed) || valueParsed <= 0)
		{
			SetError(nameof(Value), "Only numeric values allowed");
			return;
		}
		
		ClearError(nameof(Value));
	}
}