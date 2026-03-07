using RoleSystemDemo.Models;

namespace RoleSystemDemo.Messages;

/// <summary>
/// 认证状态变更消息。在以下场景通过 WeakReferenceMessenger 广播：
/// <list type="bullet">
///   <item>用户登录</item>
///   <item>用户登出</item>
///   <item>Admin 修改某用户的角色</item>
/// </list>
/// 各 ViewModel 订阅此消息以响应权限变化，无需直接耦合 AuthService。
/// </summary>
/// <param name="CurrentUser">登录后的用户；为 null 表示已登出。</param>
public sealed record AuthChangedMessage(UserModel? CurrentUser);
