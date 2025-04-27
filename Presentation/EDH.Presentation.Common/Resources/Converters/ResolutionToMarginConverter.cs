using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EDH.Presentation.Common.Resources.Converters;

public sealed class ResolutionToMarginConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not double screenWidth || screenWidth <= 0) return new Thickness(0);

		return screenWidth switch
		{
			>= 1800 => new Thickness(310,13, 310, 20),
			>= 1600 => new Thickness(280,13, 280, 20),
			>= 1400 => new Thickness(230,13, 230, 20),
			>= 1000 => new Thickness(180,13, 180, 20),
			_ => new Thickness(100,13, 100, 20)
		};
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null!;
	}
}