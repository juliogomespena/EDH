using System.Globalization;
using System.Windows.Controls;

namespace EDH.Presentation.Common.Resources.ValidationRules;

public class NotEmptyValidationRule : ValidationRule
{
	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		return string.IsNullOrWhiteSpace((value ?? "").ToString())
			? new ValidationResult(false, "Field is required.")
			: ValidationResult.ValidResult;
	}
}