using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApi.Models;

/// <summary>
/// 用户实体——映射到 Users 表
/// </summary>
[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [StringLength(500)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 个人简介
    /// </summary>
    [StringLength(200)]
    public string? Bio { get; set; }

    /// <summary>
    /// 用户角色（默认 "User"）
    /// </summary>
    [StringLength(20)]
    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 导航属性：该用户发布的文章
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    /// <summary>
    /// 导航属性：该用户的评论
    /// </summary>
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
