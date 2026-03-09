using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoleSystemDemo.Models;
using RoleSystemDemo.Services;

namespace RoleSystemDemo.ViewModels;

/// <summary>
/// 博客编辑器 ViewModel，支持新建和编辑两种模式
/// </summary>
public partial class BlogEditorViewModel : ObservableObject
{
    private readonly BlogPost? _existingPost;
    private readonly Action _navigateBack;

    public BlogEditorViewModel(BlogPost? existingPost, Action navigateBack)
    {
        _existingPost = existingPost;
        _navigateBack = navigateBack;

        IsEditMode = existingPost is not null;
        Title = existingPost?.Title ?? string.Empty;
        Content = existingPost?.Content ?? string.Empty;
    }

    public bool IsEditMode { get; }
    public string PageTitle => IsEditMode ? "编辑博客" : "写新博客";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string Content { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? ErrorMessage { get; private set; }

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var user = AuthService.Instance.CurrentUser;
        if (user is null)
        {
            ErrorMessage = "保存失败：未登录。";
            return;
        }

        var posts = DataService.LoadPosts();

        if (IsEditMode && _existingPost is not null)
        {
            var target = posts.FirstOrDefault(p => p.Id == _existingPost.Id);
            if (target is null)
            {
                ErrorMessage = "保存失败：找不到原帖子。";
                return;
            }
            target.Title = Title.Trim();
            target.Content = Content.Trim();
            target.UpdatedAt = DateTime.Now;
        }
        else
        {
            posts.Add(new BlogPost
            {
                Title = Title.Trim(),
                Content = Content.Trim(),
                AuthorId = user.Id,
                AuthorName = user.Username,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        DataService.SavePosts(posts);
        _navigateBack();
    }

    [RelayCommand]
    private void Cancel() => _navigateBack();
}
