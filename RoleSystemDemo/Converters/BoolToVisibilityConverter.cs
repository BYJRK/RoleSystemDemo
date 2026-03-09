using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RoleSystemDemo.Converters;

/// <summary>
/// 将 bool 转换为 Visibility。true → Visible，false → Collapsed
/// 设置 Invert=true 可反转（用于"无权限时显示"的提示区域）
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag = value is true;
        if (Invert) flag = !flag;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
