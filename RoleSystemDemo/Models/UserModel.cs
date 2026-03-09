namespace RoleSystemDemo.Models;

/// <summary>
/// 用户模型，对应 users.json 中的一条记录
/// </summary>
public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;

    /// <summary>SHA-256 哈希后的密码（十六进制字符串）。</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Guest;

    /// <summary>
    /// 判断当前用户是否拥有等于或高于 <paramref name="required"/> 的角色权限
    /// 权限等级：Admin > User > Guest
    /// </summary>
    public bool HasRole(UserRole required) => Role <= required;

    public override string ToString() => $"{Username} ({Role})";
}
