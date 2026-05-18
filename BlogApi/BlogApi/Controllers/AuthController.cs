using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogApi.Models;
using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IUserService userService,
        IFileService fileService,
        JwtSettings jwtSettings
    )
    {
        _userService = userService;
        _fileService = fileService;
        _jwtSettings = jwtSettings;
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// POST api/auth/register — 用户注册
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _userService.RegisterAsync(dto.Username, dto.Email, dto.Password);
        if (user == null)
            return BadRequest(ApiResponse<object>.BadRequest("用户名或邮箱已被注册"));

        var token = GenerateJwtToken(user);

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                    },
                },
                "注册成功"
            )
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.LoginAsync(dto.Username, dto.Password);
        if (user == null)
            return Unauthorized(ApiResponse<object>.BadRequest("用户名或密码错误"));

        var token = GenerateJwtToken(user);

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Avatar,
                    },
                },
                "登录成功"
            )
        );
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _userService.UpdateProfileAsync(
            userId,
            dto.Username,
            dto.Email,
            dto.Bio
        );
        if (user == null)
            return BadRequest(ApiResponse<object>.BadRequest("用户名已被占用或用户不存在"));
        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Avatar,
                    user.Bio,
                },
                "个人信息更新成功"
            )
        );
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var success = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
        if (!success)
            return BadRequest(ApiResponse<object>.BadRequest("当前密码错误"));

        return Ok(ApiResponse<object>.Ok(null, "密码修改成功"));
    }

    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsername([FromQuery] string username, [FromQuery] int? excludeUserId = null)
    {
        var isAvailable = await _userService.IsUsernameAvailableAsync(username, excludeUserId);
        return Ok(ApiResponse<object>.Ok(new { Available = isAvailable }, "检查成功"));
    }

    [HttpPost("upload-avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.BadRequest("没有文件被上传"));
        }

        const int maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
        {
            return BadRequest(ApiResponse<object>.BadRequest("文件大小不能超过 5MB"));
        }

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(
                ApiResponse<object>.BadRequest(
                    $"不支持的文件类型：{extension}，仅支持 {string.Join(", ", allowedExtensions)}"
                )
            );
        }

        string relativePath = await _fileService.SaveFileAsync(file);
        string fileUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userService.UpdateAvatarAsync(userId, fileUrl);
        if (user == null)
            return BadRequest(ApiResponse<object>.BadRequest("更新头像失败"));

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Avatar,
                    user.Bio,
                },
                "头像更新成功"
            )
        );
    }
}
