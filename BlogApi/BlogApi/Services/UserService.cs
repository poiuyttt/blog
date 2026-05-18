using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

/// <summary>
/// 用户服务实现类
/// 使用 EF Core 操作数据库
/// </summary>
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public UserService(
        ILogger<UserService> logger,
        AppDbContext context,
        JwtSettings jwtSettings
    )
    {
        _logger = logger;
        _context = context;
        _jwtSettings = jwtSettings;
    }

    public async Task<User?> RegisterAsync(string username, string email, string password)
    {
        if (await GetByUsernameAsync(username) != null)
        {
            _logger.LogWarning($"注册失败：用户名 {username} 已存在");
            return null;
        }

        if (await IsEmailTakenAsync(email))
        {
            _logger.LogWarning($"注册失败:邮箱 {email} 已被注册");
            return null;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.Now,
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"用户注册成功：{username}");

        return user;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            _logger.LogWarning($"登录失败：用户名 {username} 不存在");
            return null;
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning($"登录失败：密码错误（用户 {username}）");
            return null;
        }

        _logger.LogInformation($"用户登录成功：{username}");
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// 简易密码哈希（后续引入 BCrypt 后替换）
    /// </summary>
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(password + "BlogApiSalt")
        );
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    async Task<User?> IUserService.UpdateProfileAsync(
        int userId,
        string username,
        string email,
        string? bio
    )
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"更新失败：用户ID {userId} 不存在");
            return null;
        }

        var nameConflict = await _context.Users.AnyAsync(u =>
            u.Username == username && u.Id != userId
        );
        if (nameConflict)
        {
            _logger.LogWarning($"更新失败：用户名 {username} 已被使用");
            return null;
        }

        user.Username = username;
        user.Email = email;
        user.Bio = bio;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"用户资料更新成功：{username}");
        return user;
    }
}
