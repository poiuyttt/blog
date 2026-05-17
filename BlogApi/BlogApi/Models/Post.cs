using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApi.Models;

/// <summary>
/// 文章实体类（内部使用）
/// </summary>
[Table("Posts")]
public class Post
{
    /// <summary>
    /// 主键，数据库自增
    /// </summary>
    [Key]// 标记为主键
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// 标识为自动生成
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Summary { get; set; }

    [StringLength(100)]
    public string Author { get; set; } = "当前用户"; // 暂时固定，后续接入认证
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 评论数量（计算属性，不映射到数据库）
    /// </summary>
    [NotMapped]
    public int CommentCount => Comments?.Count ?? 0;

    /// <summary>
    /// 导航属性：本文章下的所有评论（一对多）
    /// </summary>
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    /// <summary>
    /// 分类名称（不映射到数据库）
    /// </summary>
    [NotMapped]
    public string? CategoryName => Category != null ? Category.Name : null;
}
