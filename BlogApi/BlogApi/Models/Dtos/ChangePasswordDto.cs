using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "当前密码不能为空")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "新密码长度必须在 6 到 50 个字符之间")]
    public string NewPassword { get; set; } = string.Empty;
}