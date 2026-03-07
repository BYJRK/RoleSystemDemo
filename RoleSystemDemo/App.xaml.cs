using System.Windows;

namespace RoleSystemDemo;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 首屏显示登录窗口
        new LoginWindow().Show();
    }
}
