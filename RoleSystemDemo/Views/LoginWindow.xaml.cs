using System.Windows;
using System.Windows.Controls;
using RoleSystemDemo.ViewModels;

namespace RoleSystemDemo;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();

        DataContext = new LoginViewModel(onLoginSuccess: () =>
        {
            // 登录成功：打开主窗口，关闭自身
            var main = new MainWindow();
            main.Show();
            Close();
        });
    }

    // WPF 的 PasswordBox.Password 不支持双向绑定（安全设计），
    // 在 code-behind 手动同步到 ViewModel。
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
            vm.Password = ((PasswordBox)sender).Password;
    }
}
