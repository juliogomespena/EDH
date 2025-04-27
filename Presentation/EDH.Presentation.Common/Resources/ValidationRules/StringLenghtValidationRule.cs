using System.Globalization;
using System.Windows.Controls;

namespace EDH.Presentation.Common.Resources.ValidationRules;

public class StringLengthValidationRule : ValidationRule
{
	public int MinimumLength { get; set; } = 1; 
	public int MaximumLength { get; set; } = int.MaxValue;
	public string ErrorMessage { get; set; } = "Field is required.";
	public bool AllowEmpty { get; set; } = false; 

	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		string? stringValue = (value ?? "").ToString();

		if (AllowEmpty && String.IsNullOrEmpty(stringValue))
		{
			return ValidationResult.ValidResult;
		}

		if (String.IsNullOrWhiteSpace(stringValue))
		{
			return new ValidationResult(false, ErrorMessage);
		}

		if (stringValue.Length < MinimumLength)
		{
			string error = MinimumLength == 1
				? ErrorMessage
				: $"Minimum length is {MinimumLength} characters.";
			return new ValidationResult(false, error);
		}

		if (stringValue.Length > MaximumLength)
		{
			return new ValidationResult(false, $"Maximum length is {MaximumLength} characters.");
		}

		return ValidationResult.ValidResult;
	}
}