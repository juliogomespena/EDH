using System.Globalization;
using System.Windows.Data;
using EDH.Core.Enums;

namespace EDH.Presentation.Common.Resources.Converters;

public sealed class DiscountSurchargeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DiscountSurcharge discountSurcharge)
        {
            return discountSurcharge switch
            {
                DiscountSurcharge.Money => "$",
                DiscountSurcharge.Percentage => "%",
                _ => "$"
            };
        }
        return "$";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            return stringValue switch
            {
                "$" => DiscountSurcharge.Money,
                "%" => DiscountSurcharge.Percentage,
                _ => DiscountSurcharge.Money
            };
        }
        return DiscountSurcharge.Money;
    }

}