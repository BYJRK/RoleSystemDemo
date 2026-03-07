namespace RoleSystemDemo.Models;

/// <summary>
/// 表示用户角色。角色决定了用户在系统中的权限范围。
/// </summary>
public enum UserRole
{
    /// <summary>管理员：拥有全部权限，可管理所有博客和用户。</summary>
    Admin,

    /// <summary>普通用户：可以增删改自己的博客。</summary>
    User,

    /// <summary>访客：只能阅读博客，无法进行任何写操作。</summary>
    Guest
}
