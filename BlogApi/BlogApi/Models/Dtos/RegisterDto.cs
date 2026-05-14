using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos
{
    /// <summary>
    /// 注册请求体
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码至少6位")]
        public string Password { get; set; } = string.Empty;
    }
}
