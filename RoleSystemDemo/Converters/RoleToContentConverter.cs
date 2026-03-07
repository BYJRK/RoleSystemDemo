using System.Globalization;
using System.Windows.Data;
using RoleSystemDemo.Models;

namespace RoleSystemDemo.Converters;

/// <summary>
/// 【权限模式三：动态内容型】
/// 将当前用户角色转换为字符串提示文字。
/// 权限足够时返回 null（绑定目标可用 Visibility 的 FallbackValue 处理）；
/// 权限不足时返回提示字符串，显示在 TextBlock 等控件中。
///
/// ConverterParameter 格式：
///   "AllowedRoles|足够时文字|不足时文字"
///
/// 用法示例（Guest 显示登录提示，其他角色显示正常功能标签）：
/// <code>
/// Text="{Binding Source={x:Static services:AuthService.Instance},
///     Path=CurrentRole,
///     Converter={StaticResource RoleToContentConverter},
///     ConverterParameter='Admin|User|💬 参与讨论|🔒 登录后参与讨论'}"
/// </code>
/// </summary>
[ValueConversion(typeof(UserRole), typeof(string))]
public sealed class RoleToContentConverter : IValueConverter
{
    /// <summary>
    /// ConverterParameter 格式：<c>允许角色（|分隔）§有权限时的内容§无权限时的内容</c>，
    /// 三段之间用 § 分隔（避免与角色名的 | 冲突）。
    /// </summary>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not UserRole role) return null;
        if (parameter is not string param) return null;

        // 格式：AllowedRoles§ContentIfAllowed§ContentIfDenied
        var parts = param.Split('§');
        if (parts.Length < 3) return param; // 格式不对时原样返回

        var allowedRoles = parts[0];
        var contentIfAllowed = parts[1];
        var contentIfDenied = parts[2];

        return RoleToVisibilityConverter.IsAllowed(role, allowedRoles)
            ? contentIfAllowed
            : contentIfDenied;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}
