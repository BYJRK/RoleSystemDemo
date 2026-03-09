namespace RoleSystemDemo.Models;

/// <summary>
/// 博客文章模型，对应 posts.json 中的一条记录
/// </summary>
public class BlogPost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    /// <summary>作者的用户 ID，用于判断所有权。</summary>
    public Guid AuthorId { get; set; }

    /// <summary>冗余存储的作者名，避免每次查询用户表。</summary>
    public string AuthorName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
