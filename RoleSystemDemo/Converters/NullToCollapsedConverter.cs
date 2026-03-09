using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RoleSystemDemo.Converters;

/// <summary>
/// null 或空字符串 → Collapsed；非 null 非空 → Visible
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public sealed class NullToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string s
            ? (string.IsNullOrEmpty(s) ? Visibility.Collapsed : Visibility.Visible)
            : (value is null ? Visibility.Collapsed : Visibility.Visible);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
