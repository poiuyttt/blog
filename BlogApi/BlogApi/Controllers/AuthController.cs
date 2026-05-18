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
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IUserService userService,
        JwtSettings jwtSettings
    )
    {
        _userService = userService;
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

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    user.Id,
                    user.Username,
                    user.Email,
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
                    user.Bio,
                },
                "个人信息更新成功"
            )
        );
    }
}
