using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos;

public class CreateCommentDto
{
    [Required(ErrorMessage = "文章ID不能为空")]
    public int PostId { get; set; }

    [Required(ErrorMessage = "评论内容不能为空")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "评论内容长度必须在1-500之间")]
    public string Comment { get; set; } = string.Empty;
}
