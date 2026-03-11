# RoleSystemDemo

一个用于教学的 WPF 演示项目，展示如何在 MVVM 架构下实现基于角色的 UI 权限系统。

B 站视频链接：<https://www.bilibili.com/video/BV1uscfzNEaU>

## 项目演示内容

项目模拟一个简单的博客平台，包含三种用户角色：

| 角色 | 说明 |
|------|------|
| Admin | 可管理所有用户和帖子 |
| User | 可发布和管理自己的帖子 |
| Guest | 仅可浏览，无编辑权限 |

重点演示三种 UI 权限表达模式：

1. **隐藏型** — 权限不足时控件完全不可见（如"新建帖子"按钮对 Guest 不显示）
2. **禁用型** — 控件始终可见，但权限不足时置灰，并通过 Tooltip 给出说明
3. **动态内容型** — 同一区域根据角色呈现完全不同的内容（如详情页底部互动区域）

## 技术思路

- **框架**：WPF / .NET，使用原生 Fluent 主题
- **MVVM**：基于 [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)，用 `[ObservableProperty]`、`[RelayCommand]` 等 Source Generator 减少样板代码
- **认证**：`AuthService` 单例维护当前登录用户，无需 DI 容器
- **消息通信**：角色变更时通过 `WeakReferenceMessenger` 广播 `AuthChangedMessage`，ViewModel 订阅后刷新自身权限状态，实现发送方与接收方解耦
- **数据持久化**：使用 `System.Text.Json` 将用户和帖子存储为本地 JSON 文件，无需数据库
- **导航**：ViewModel-first 导航，`MainViewModel` 切换 `CurrentViewModel`，通过 `DataTemplate` 自动映射对应 View

## 运行

```shell
dotnet run --project RoleSystemDemo
```

或者可以安装 [TaskFile](https://taskfile.dev/) 后直接运行：

```shell
task run
```
