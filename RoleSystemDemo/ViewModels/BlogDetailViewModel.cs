using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// 帖子详情页 ViewModel。
/// 演示【模式三：动态内容】：底部互动区域根据当前角色显示不同内容。
/// </summary>
public partial class BlogDetailViewModel : ObservableObject
{
    private readonly Action _navigateBack;

    public BlogDetailViewModel(BlogPost post, Action navigateBack)
    {
        Post = post;
        _navigateBack = navigateBack;
        RefreshInteractionArea();
    }

    public BlogPost Post { get; }

    // ── 基础展示 ───────────────────────────────────────────────────────────

    public string Title => Post.Title;
    public string Content => Post.Content;
    public string AuthorName => Post.AuthorName;
    public string PublishedAt => Post.CreatedAt.ToString("yyyy年MM月dd日 HH:mm");
    public string UpdatedAt => Post.UpdatedAt != Post.CreatedAt
        ? $"（更新于 {Post.UpdatedAt:yyyy-MM-dd HH:mm}）"
        : string.Empty;

    // ── 互动区域（演示动态内容）────────────────────────────────────────────

    /// <summary>
    /// 【模式三：动态内容】底部互动区域的标题，随角色不同而变化。
    /// </summary>
    [ObservableProperty]
    public partial string InteractionTitle { get; private set; } = string.Empty;

    /// <summary>
    /// 【模式三：动态内容】互动区域的说明文字，随角色不同而变化。
    /// </summary>
    [ObservableProperty]
    public partial string InteractionHint { get; private set; } = string.Empty;

    /// <summary>
    /// 互动区域的背景标记，用于 XAML 中根据角色绑定不同的视觉样式。
    /// </summary>
    [ObservableProperty]
    public partial UserRole InteractionRole { get; private set; }

    private void RefreshInteractionArea()
    {
        var role = AuthService.Instance.CurrentRole;
        InteractionRole = role;

        (InteractionTitle, InteractionHint) = role switch
        {
            UserRole.Admin => (
                "🛡️ 管理员视图",
                "作为管理员，您可以在「Admin Panel」中删除任意帖子或管理用户权限。"
            ),
            UserRole.User => (
                "💬 参与讨论",
                "您可以在此发表评论（功能演示占位）。如果这是您的帖子，可点击「编辑」进行修改。"
            ),
            _ => (
                "🔒 加入讨论",
                "登录后即可参与讨论、发表评论并发布自己的博客。请使用 User 或 Admin 账号登录。"
            )
        };
    }

    // ── 命令 ───────────────────────────────────────────────────────────────

    [RelayCommand]
    private void Back() => _navigateBack();
}
