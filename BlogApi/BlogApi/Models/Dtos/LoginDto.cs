using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(20, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(20, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
