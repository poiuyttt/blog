using BlogApi.Models;

namespace BlogApi.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <returns>注册成功的用户对象，失败返回 null</returns>
        Task<User?> RegisterAsync(string username, string email, string password);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <returns>验证成功返回用户对象，失败返回 null</returns>
        Task<User?> LoginAsync(string username, string password);

        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// 检查邮箱是否已被注册
        /// </summary>
        Task<bool> IsEmailTakenAsync(string email);

        /// <summary>
        /// 更新用户个人信息
        /// </summary>
        Task<User?> UpdateProfileAsync(int userId, string username, string email, string? bio);
    }
}
