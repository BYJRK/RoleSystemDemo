using CommunityToolkit.Mvvm.Messaging;
using RoleSystemDemo.Messages;
using RoleSystemDemo.Models;

namespace RoleSystemDemo.Services;

/// <summary>
/// 认证服务（单例）
/// 负责登录验证、会话管理，并在状态变更时通过 <see cref="WeakReferenceMessenger"/>
/// 广播 <see cref="AuthChangedMessage"/>，使所有订阅的 ViewModel 能响应权限变化
/// </summary>
public sealed class AuthService
{
    #region 单例

    public static AuthService Instance { get; } = new();

    private AuthService() { }

    #endregion

    #region 状态

    /// <summary>当前登录的用户；未登录时为 null。</summary>
    public UserModel? CurrentUser { get; private set; }

    /// <summary>当前用户的角色；未登录时为 <see cref="UserRole.Guest"/>。</summary>
    public UserRole CurrentRole => CurrentUser?.Role ?? UserRole.Guest;

    /// <summary>是否已登录。</summary>
    public bool IsLoggedIn => CurrentUser is not null;

    #endregion

    #region 操作

    /// <summary>
    /// 尝试用用户名和明文密码登录
    /// </summary>
    /// <returns>登录成功返回 true，否则 false。</returns>
    public bool Login(string username, string password)
    {
        var users = DataService.LoadUsers();
        var hash = PasswordService.ComputeHash(password);
        var user = users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && u.PasswordHash == hash
        );

        if (user is null)
            return false;

        CurrentUser = user;
        Broadcast();
        return true;
    }

    /// <summary>登出当前用户。</summary>
    public void Logout()
    {
        CurrentUser = null;
        Broadcast();
    }

    /// <summary>
    /// 通知 AuthService 当前用户的角色已在外部被修改（例如 Admin 面板改角色），
    /// 触发一次 <see cref="AuthChangedMessage"/> 广播以刷新全局 UI
    /// </summary>
    public void NotifyRoleChanged(UserModel updatedUser)
    {
        // 如果修改的是当前登录用户，同步更新内存中的对象
        if (CurrentUser?.Id == updatedUser.Id)
            CurrentUser = updatedUser;

        Broadcast();
    }

    #endregion

    #region 内部

    private void Broadcast() =>
        WeakReferenceMessenger.Default.Send(new AuthChangedMessage(CurrentUser));

    #endregion
}
