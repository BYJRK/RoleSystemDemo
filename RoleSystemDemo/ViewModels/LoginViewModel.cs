using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// 登录窗口 ViewModel。
/// 登录成功后通过回调 <see cref="OnLoginSuccess"/> 通知外部（App）切换主窗口。
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly Action _onLoginSuccess;

    public LoginViewModel(Action onLoginSuccess)
    {
        _onLoginSuccess = onLoginSuccess;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasError { get; private set; }

    private bool CanLogin() =>
        !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private void Login()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (AuthService.Instance.Login(Username, Password))
        {
            _onLoginSuccess();
        }
        else
        {
            HasError = true;
            ErrorMessage = "用户名或密码错误，请重试。";
        }
    }
}
