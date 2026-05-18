namespace BlogApi.Models.Dtos;

public class PostListDto
{
    /// <summary>
    /// 文章 ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 摘要
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 评论数（通过关联查询统计）
    /// </summary>
    public int CommentCount { get; set; }

    public string? CategoryName { get; set; }
}
