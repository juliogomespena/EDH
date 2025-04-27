using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EDH.Presentation.Common.Resources.Converters;

public class PositiveNegativeToBrushConverter : IValueConverter
{
	public Brush PositiveBrush { get; set; }
	public Brush NegativeBrush { get; set; }
	public Brush ZeroBrush { get; set; }

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not decimal d) return ZeroBrush;

		if (d > 0) return PositiveBrush;
		if (d < 0) return NegativeBrush;

		return ZeroBrush;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> throw new NotImplementedException();
}