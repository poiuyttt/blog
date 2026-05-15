using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos;

/// <summary>
/// 创建文章的请求体 DTO
/// 使用数据注解声明验证规则
/// </summary>
///
public class CreatePostDto
{
    /// <summary>
    /// 文章标题
    /// </summary>
    // [Required]：必填，不能为 null 或空字符串
    // ErrorMessage：自定义错误提示信息
    [Required(ErrorMessage = "文章标题不能为空")]
    // [StringLength]：字符串最大长度
    [StringLength(100, MinimumLength = 3, ErrorMessage = "标题长度必须在3-100个字符之间")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 文章内容（Markdown 格式）
    /// </summary>
    [Required(ErrorMessage = "文章内容不能为空")]
    [MinLength(10, ErrorMessage = "文章内容最少需要10个字符")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 文章摘要
    /// </summary>
    [StringLength(200, ErrorMessage = "摘要长度必须在200个字符以内")]
    public string? Summary { get; set; } // string? 表示此字段可选

    [Required(ErrorMessage = "作者名称不能为空")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "作者名称长度必须在3-100个字符之间")]
    public string Author { get; set; } = string.Empty;
}