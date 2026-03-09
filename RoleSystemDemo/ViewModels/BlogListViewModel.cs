using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RoleSystemDemo.Messages;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// 博客列表页 ViewModel
/// 演示三种 UI 权限模式在列表场景中的实际应用
/// </summary>
public partial class BlogListViewModel : ObservableRecipient, IRecipient<AuthChangedMessage>
{
    private readonly Action<BlogPost> _navigateToDetail;
    private readonly Action<BlogPost?> _navigateToEditor;

    public BlogListViewModel(
        Action<BlogPost> navigateToDetail,
        Action<BlogPost?> navigateToEditor)
    {
        _navigateToDetail = navigateToDetail;
        _navigateToEditor = navigateToEditor;
        IsActive = true;
        LoadPosts();
        RefreshHeaderState();
    }

    #region 帖子列表

    public ObservableCollection<BlogPostItemViewModel> Posts { get; } = [];

    private void LoadPosts()
    {
        Posts.Clear();
        var posts = DataService.LoadPosts().OrderByDescending(p => p.CreatedAt);
        foreach (var post in posts)
        {
            Posts.Add(new BlogPostItemViewModel(
                post,
                onEdit: p => _navigateToEditor(p),
                onDelete: DeletePost,
                onView: p => _navigateToDetail(p)));
        }
    }

    private void DeletePost(BlogPost post)
    {
        var all = DataService.LoadPosts();
        all.RemoveAll(p => p.Id == post.Id);
        DataService.SavePosts(all);
        LoadPosts();
    }

    #endregion

    #region 新建帖子区域

    /// <summary>
    /// “新建帖子”按钮本体：仅 User 和 Admin 可见
    /// </summary>
    [ObservableProperty]
    public partial bool CanCreatePost { get; private set; }

    /// <summary>
    /// 按钮是否启用（始终显示，权限不足时置灰）
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NewPostCommand))]
    public partial bool NewPostEnabled { get; private set; }

    /// <summary>
    /// 写在“新建帖子”区域旁的提示文字
    /// 权限不足时提示如何升级权限，有权限时为 null
    /// </summary>
    [ObservableProperty]
    public partial string? NewPostHint { get; private set; }

    /// <summary>
    /// “新建帖子”禁用按钮的悬停提示
    /// </summary>
    [ObservableProperty]
    public partial string NewPostButtonTooltip { get; private set; } = string.Empty;

    [RelayCommand(CanExecute = nameof(NewPostEnabled))]
    private void NewPost() => _navigateToEditor(null);

    #endregion

    #region 消息接收

    public void Receive(AuthChangedMessage message)
    {
        RefreshHeaderState();
        // 刷新每条帖子的权限状态
        foreach (var item in Posts)
            item.RefreshPermissions();
    }

    private void RefreshHeaderState()
    {
        var role = AuthService.Instance.CurrentRole;

        CanCreatePost = role is UserRole.Admin or UserRole.User;
        NewPostEnabled = CanCreatePost;

        NewPostHint = role switch
        {
            UserRole.Guest => "⚠️ 仅注册用户可发布博客，请使用 User 或 Admin 账号登录",
            _              => null
        };

        NewPostButtonTooltip = role switch
        {
            UserRole.Guest => "权限不足：请登录 User 或 Admin 账号后发帖",
            _              => "发布一篇新博客"
        };
    }

    #endregion
}
