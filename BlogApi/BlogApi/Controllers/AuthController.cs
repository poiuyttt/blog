using BlogApi.Models;
using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private ILogger<AuthController> _logger;
        private JwtSettings _jwtSettings;

        public AuthController(
            IUserService userService,
            ILogger<AuthController> logger,
            JwtSettings jwtSettings
        )
        {
            _userService = userService;
            _logger = logger;
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

            _logger.LogInformation($"新用户注册：{user.Username}");
            return Ok(
                new
                {
                    Message = "注册成功",
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                    },
                }
            );
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.LoginAsync(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized(new { Message = "用户名或密码错误" });

            var token = GenerateJwtToken(user);
            _logger.LogInformation($"用户登录：{user.Username}");

            return Ok(
                new
                {
                    Message = "登陆成功",
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Avatar,
                    },
                }
            );
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            int userId = 1;

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
}
