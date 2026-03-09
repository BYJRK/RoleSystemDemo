using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RoleSystemDemo.Messages;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// 主窗口 Shell ViewModel
/// 负责导航管理（ViewModel-first）以及顶部导航栏的状态
/// 订卫 <see cref="AuthChangedMessage"/> 以响应登录/登出/角色变更
/// </summary>
public partial class MainViewModel : ObservableRecipient, IRecipient<AuthChangedMessage>
{
    public MainViewModel()
    {
        IsActive = true; // 激活消息接收
        RefreshAuthState(AuthService.Instance.CurrentUser);
        NavigateToBlogList();
    }

    #region 导航

    /// <summary>当前显示的子 ViewModel；通过 DataTemplate 自动匹配对应的 View。</summary>
    [ObservableProperty]
    public partial object? CurrentViewModel { get; private set; }

    [RelayCommand]
    private void NavigateToBlogList()
    {
        CurrentViewModel = new BlogListViewModel(NavigateToDetail, NavigateToEditor);
    }

    [RelayCommand(CanExecute = nameof(IsAdmin))]
    private void NavigateToAdmin()
    {
        CurrentViewModel = new AdminViewModel();
    }

    private void NavigateToDetail(BlogPost post)
    {
        CurrentViewModel = new BlogDetailViewModel(post, NavigateToBlogList);
    }

    private void NavigateToEditor(BlogPost? post)
    {
        CurrentViewModel = new BlogEditorViewModel(post, NavigateToBlogList);
    }

    #endregion

    #region 认证状态

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RoleBadge))]
    [NotifyPropertyChangedFor(nameof(LogoutButtonText))]
    [NotifyCanExecuteChangedFor(nameof(NavigateToAdminCommand))]
    public partial UserRole CurrentRole { get; private set; }

    [ObservableProperty]
    public partial string CurrentUsername { get; private set; } = "访客";

    /// <summary>登出/返回登录按钮的文字：已登录时显示"登出"，访客模式显示"返回登录"。</summary>
    public string LogoutButtonText => CurrentRole == UserRole.Guest ? "返回登录" : "登出";

    public bool IsAdmin => CurrentRole == UserRole.Admin;

    /// <summary>角色徽章文字，显示在顶部导航栏。</summary>
    public string RoleBadge => CurrentRole switch
    {
        UserRole.Admin => "👑 Admin",
        UserRole.User  => "✏️ User",
        _              => "👁 Guest"
    };

    #endregion

    #region 登出

    /// <summary>
    /// 登出命令。调用 AuthService 登出，由 Messenger 广播触发 App 层切换回登录窗口
    /// </summary>
    [RelayCommand]
    private void Logout() => AuthService.Instance.Logout();

    #endregion

    #region 消息接收

    /// <summary>
    /// 收到 <see cref="AuthChangedMessage"/> 时刷新导航栏状态
    /// 登出时（CurrentUser == null）由 App 层处理窗口切换
    /// </summary>
    public void Receive(AuthChangedMessage message)
    {
        RefreshAuthState(message.CurrentUser);

        // 如果角色变更导致当前页面不再可访问，回退到列表
        if (CurrentViewModel is AdminViewModel && !IsAdmin)
            NavigateToBlogList();
    }

    private void RefreshAuthState(UserModel? user)
    {
        CurrentUsername = user?.Username ?? "访客";
        CurrentRole = user?.Role ?? UserRole.Guest;
    }

    #endregion
}
