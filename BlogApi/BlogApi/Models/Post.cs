namespace BlogApi.Models;

/// <summary>
/// 文章实体类（内部使用）
/// </summary>
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Author { get; set; } = "当前用户"; // 暂时固定，后续接入认证
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}