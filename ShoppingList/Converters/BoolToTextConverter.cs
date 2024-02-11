using System.Globalization;

namespace ShoppingList.Converters;

public class BoolToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolean)
        {
            return boolean ? "Yes" : "No";
        }

        throw new NotImplementedException("Converter must not be used with a non-boolean value");
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is string stringValue)
        {
            return stringValue.Equals("Yes", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}
