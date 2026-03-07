using System.Globalization;
using System.Windows.Data;
using RoleSystemDemo.Models;

namespace RoleSystemDemo.Converters;

/// <summary>
/// 【权限模式二：禁用型】
/// 将当前用户角色转换为 <see cref="bool"/>（IsEnabled）。
/// 权限不足时返回 false（控件可见但置灰），配合 ToolTip 给出说明。
///
/// 用法示例（User 或 Admin 才能启用此按钮，Guest 看到置灰按钮）：
/// <code>
/// IsEnabled="{Binding Source={x:Static services:AuthService.Instance},
///     Path=CurrentRole,
///     Converter={StaticResource RoleToEnabledConverter},
///     ConverterParameter=Admin|User}"
/// </code>
/// </summary>
[ValueConversion(typeof(UserRole), typeof(bool))]
public sealed class RoleToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not UserRole role) return false;
        return RoleToVisibilityConverter.IsAllowed(role, parameter?.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
