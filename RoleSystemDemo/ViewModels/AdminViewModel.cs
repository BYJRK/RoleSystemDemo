using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// Admin 面板 ViewModel
/// 包含用户管理（修改角色、削除用户）和帖子管理（削除任意帖子）两个 Tab
/// </summary>
public partial class AdminViewModel : ObservableObject
{
    public AdminViewModel()
    {
        LoadData();
    }

    #region 用户管理

    public ObservableCollection<UserAdminItemViewModel> Users { get; } = [];

    #endregion

    #region 帖子管理

    public ObservableCollection<PostAdminItemViewModel> AllPosts { get; } = [];

    private void LoadData()
    {
        LoadUsers();
        LoadPosts();
    }

    private void LoadUsers()
    {
        Users.Clear();
        foreach (var u in DataService.LoadUsers())
            Users.Add(new UserAdminItemViewModel(u, OnRoleChanged, DeleteUser));
    }

    private void LoadPosts()
    {
        AllPosts.Clear();
        var users = DataService.LoadUsers();
        foreach (var p in DataService.LoadPosts().OrderByDescending(p => p.CreatedAt))
            AllPosts.Add(new PostAdminItemViewModel(p, DeletePost));
    }

    private void OnRoleChanged(UserModel user)
    {
        var all = DataService.LoadUsers();
        var target = all.FirstOrDefault(u => u.Id == user.Id);
        if (target is null) return;

        target.Role = user.Role;
        DataService.SaveUsers(all);

        // 通知 AuthService（如果修改的是当前登录用户，同步刷新全局权限）
        AuthService.Instance.NotifyRoleChanged(target);
    }

    private void DeleteUser(UserModel user)
    {
        var all = DataService.LoadUsers();
        all.RemoveAll(u => u.Id == user.Id);
        DataService.SaveUsers(all);
        LoadUsers();
    }

    private void DeletePost(BlogPost post)
    {
        var all = DataService.LoadPosts();
        all.RemoveAll(p => p.Id == post.Id);
        DataService.SavePosts(all);
        LoadPosts();
    }

    #endregion
}

#region 嵌套行 ViewModel

/// <summary>用户管理列表的单行 ViewModel。</summary>
public partial class UserAdminItemViewModel : ObservableObject
{
    private readonly Action<UserModel> _onRoleChanged;
    private readonly Action<UserModel> _onDelete;

    public UserAdminItemViewModel(
        UserModel user,
        Action<UserModel> onRoleChanged,
        Action<UserModel> onDelete)
    {
        User = user;
        SelectedRole = user.Role;
        _onRoleChanged = onRoleChanged;
        _onDelete = onDelete;

        // 不允许修改当前登录的 Admin 自身的角色（防止自我降级）
        CanModify = AuthService.Instance.CurrentUser?.Id != user.Id;
    }

    public UserModel User { get; }
    public string Username => User.Username;
    public bool CanModify { get; }

    public Array AllRoles { get; } = Enum.GetValues(typeof(UserRole));

    [ObservableProperty]
    public partial UserRole SelectedRole { get; set; }

    [RelayCommand]
    private void SaveRole()
    {
        User.Role = SelectedRole;
        _onRoleChanged(User);
    }

    [RelayCommand]
    private void Delete() => _onDelete(User);
}

/// <summary>帖子管理列表的单行 ViewModel。</summary>
public partial class PostAdminItemViewModel : ObservableObject
{
    private readonly Action<BlogPost> _onDelete;

    public PostAdminItemViewModel(BlogPost post, Action<BlogPost> onDelete)
    {
        Post = post;
        _onDelete = onDelete;
    }

    public BlogPost Post { get; }
    public string Title => Post.Title;
    public string AuthorName => Post.AuthorName;
    public string CreatedAt => Post.CreatedAt.ToString("yyyy-MM-dd");

    [RelayCommand]
    private void Delete() => _onDelete(Post);
}

#endregion
