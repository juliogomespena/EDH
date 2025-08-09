using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EDH.Presentation.Common.Resources.Converters;

public sealed class ResolutionToMarginConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
	{
		if (value is not double screenWidth || screenWidth <= 0) return new Thickness(0);

		return screenWidth switch
		{
			>= 1600 => new Thickness(250,13, 250, 20),
			>= 1100 => new Thickness(190,13, 190, 20),
			>= 900 => new Thickness(100,13, 100, 20),
			_ => new Thickness(70,13, 70, 20)
		};
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
	{
		return null!;
	}
}