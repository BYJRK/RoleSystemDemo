using System.Globalization;
using System.Windows;
using System.Windows.Data;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.Converters;

/// <summary>
/// 【权限模式一：隐藏型】
/// 将当前用户角色转换为 <see cref="Visibility"/>
/// 权限不足时返回 <see cref="Visibility.Collapsed"/>（控件完全不显示）
///
/// 用法示例（只有 Admin 能看到此控件）：
/// <code>
/// Visibility="{Binding Source={x:Static services:AuthService.Instance},
///     Path=CurrentRole,
///     Converter={StaticResource RoleToVisibilityConverter},
///     ConverterParameter=Admin}"
/// </code>
/// 多角色用 | 分隔（如 Admin|User）：
/// <code>ConverterParameter=Admin|User</code>
/// </summary>
[ValueConversion(typeof(UserRole), typeof(Visibility))]
public sealed class RoleToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not UserRole role) return Visibility.Collapsed;
        return IsAllowed(role, parameter?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    internal static bool IsAllowed(UserRole role, string? parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter)) return true;
        var allowed = parameter.Split('|', StringSplitOptions.RemoveEmptyEntries);
        return allowed.Any(a => Enum.TryParse<UserRole>(a.Trim(), out var r) && r == role);
    }
}
