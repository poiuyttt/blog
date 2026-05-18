using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Bio { get; set; }
}
