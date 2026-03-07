using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// 博客列表中单条帖子的行 ViewModel。
/// 封装权限相关的三种 UI 表达模式：
/// <list type="bullet">
///   <item><see cref="CanDelete"/> — 隐藏型：无权限时完全不显示删除按钮</item>
///   <item><see cref="CanEdit"/> — 禁用型：即使无权限仍显示编辑按钮，但置灰并给出 Tooltip</item>
///   <item><see cref="EditTooltip"/> — 动态内容：根据具体原因显示不同提示</item>
/// </list>
/// </summary>
public partial class BlogPostItemViewModel : ObservableObject
{
    public BlogPost Post { get; }

    private readonly Action<BlogPost> _onEdit;
    private readonly Action<BlogPost> _onDelete;
    private readonly Action<BlogPost> _onView;

    public BlogPostItemViewModel(
        BlogPost post,
        Action<BlogPost> onEdit,
        Action<BlogPost> onDelete,
        Action<BlogPost> onView)
    {
        Post = post;
        _onEdit = onEdit;
        _onDelete = onDelete;
        _onView = onView;
        RefreshPermissions();
    }

    // ── 显示属性 ───────────────────────────────────────────────────────────

    public string Title => Post.Title;
    public string AuthorName => Post.AuthorName;
    public string CreatedAt => Post.CreatedAt.ToString("yyyy-MM-dd");
    public string Summary => Post.Content.Length > 80
        ? Post.Content[..80] + "…"
        : Post.Content;

    // ── 权限属性（教学重点）────────────────────────────────────────────────

    /// <summary>
    /// 【模式一：禁用型】编辑按钮始终可见，但仅在有权限时可用。
    /// 权限规则：管理员可编辑任何帖子；普通用户只能编辑自己的帖子。
    /// </summary>
    [ObservableProperty]
    public partial bool CanEdit { get; private set; }

    /// <summary>
    /// 【模式一配套 Tooltip】权限不足时给出具体原因说明。
    /// </summary>
    [ObservableProperty]
    public partial string EditTooltip { get; private set; } = string.Empty;

    /// <summary>
    /// 【模式一：禁用型】删除按钮始终可见，但仅在有权限时可用。
    /// 权限规则：管理员可删除任何帖子；普通用户只能删除自己的帖子。
    /// </summary>
    [ObservableProperty]
    public partial bool CanDelete { get; private set; }

    /// <summary>
    /// 【模式一配套 Tooltip】权限不足时给出具体原因说明。
    /// </summary>
    [ObservableProperty]
    public partial string DeleteTooltip { get; private set; } = string.Empty;

    // ── 命令 ───────────────────────────────────────────────────────────────

    [RelayCommand]
    private void View() => _onView(Post);

    [RelayCommand]
    private void Edit() => _onEdit(Post);

    [RelayCommand]
    private void Delete() => _onDelete(Post);

    // ── 权限刷新（收到 AuthChangedMessage 后由父 ViewModel 调用）────────────

    public void RefreshPermissions()
    {
        var user = AuthService.Instance.CurrentUser;
        var role = AuthService.Instance.CurrentRole;
        bool isOwner = user?.Id == Post.AuthorId;

        // 编辑权限
        CanEdit = role switch
        {
            UserRole.Admin => true,
            UserRole.User  => isOwner,
            _              => false
        };

        EditTooltip = CanEdit ? "编辑此帖子" : role switch
        {
            UserRole.User  => "只能编辑自己的帖子",
            _              => "请登录 User 账号后编辑帖子"
        };

        // 删除权限
        CanDelete = role switch
        {
            UserRole.Admin => true,
            UserRole.User  => isOwner,
            _              => false
        };

        DeleteTooltip = CanDelete ? "删除此帖子" : role switch
        {
            UserRole.User  => "只能删除自己的帖子",
            _              => "请登录 User 账号后删除帖子"
        };
    }
}
