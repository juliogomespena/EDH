using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EDH.Presentation.Common.Resources.Converters;

public sealed class PositiveNegativeToBrushConverter : IValueConverter
{
	public Brush PositiveBrush { get; set; }
	public Brush NegativeBrush { get; set; }
	public Brush ZeroBrush { get; set; }

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is not IConvertible convertible) return ZeroBrush;

		try
		{
			double numericValue = convertible.ToDouble(culture);

			return numericValue switch
			{
				> 0 => PositiveBrush,
				< 0 => NegativeBrush,
				_ => ZeroBrush
			};
		}
		catch (InvalidCastException)
		{
			return ZeroBrush;
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> throw new NotImplementedException();
}