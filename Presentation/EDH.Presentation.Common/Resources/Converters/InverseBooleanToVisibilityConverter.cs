using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace EDH.Presentation.Common.Resources.Converters;

public class InverseBooleanToVisibilityConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool boolean)
		{
			return boolean ? Visibility.Collapsed : Visibility.Visible;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is Visibility visibility)
		{
			return visibility != Visibility.Visible;
		}
		return true;
	}
}
