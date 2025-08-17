using System.Globalization;
using System.Windows.Data;
using EDH.Core.Enums;

namespace EDH.Presentation.Common.Resources.Converters;

public sealed class DiscountSurchargeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DiscountSurchargeMode discountSurcharge)
        {
            return discountSurcharge switch
            {
                DiscountSurchargeMode.Money => "$",
                DiscountSurchargeMode.Percentage => "%",
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
                "$" => DiscountSurchargeMode.Money,
                "%" => DiscountSurchargeMode.Percentage,
                _ => DiscountSurchargeMode.Money
            };
        }
        return DiscountSurchargeMode.Money;
    }

}