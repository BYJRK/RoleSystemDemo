using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using RoleSystemDemo.Messages;
using RoleSystemDemo.ViewModels;

namespace RoleSystemDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        // 监听登出消息：收到后关闭主窗口，重新打开登录窗口
        WeakReferenceMessenger.Default.Register<AuthChangedMessage>(this, (_, msg) =>
        {
            if (msg.CurrentUser is null)
            {
                Dispatcher.Invoke(() =>
                {
                    var login = new LoginWindow();
                    login.Show();
                    Close();
                });
            }
        });
    }

    protected override void OnClosed(System.EventArgs e)
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        base.OnClosed(e);
    }
}
