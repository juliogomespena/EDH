using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace EDH.Presentation.Common.Resources.ValidationRules;

public class RegexValidationRule : ValidationRule
{
	public string Pattern { get; set; } = String.Empty;
	public string ErrorMessage { get; set; } = "Input format is invalid.";
	public bool AllowEmpty { get; set; } = false;

	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		string? stringValue = (value ?? "").ToString();

		if (AllowEmpty && String.IsNullOrEmpty(stringValue))
		{
			return ValidationResult.ValidResult;
		}

		if (String.IsNullOrWhiteSpace(Pattern))
		{
			return ValidationResult.ValidResult;
		}

		if (!Regex.IsMatch(stringValue, Pattern))
		{
			return new ValidationResult(false, ErrorMessage);
		}

		return ValidationResult.ValidResult;
	}
}