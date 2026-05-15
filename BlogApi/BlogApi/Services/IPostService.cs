using BlogApi.Models;

namespace BlogApi.Services;

/// <summary>
/// 文章服务接口
/// </summary>
public interface IPostService
{
    // IEnumerable<Post>：返回文章集合（只读）
    Task<IEnumerable<Post>> GetAllAsync();

    Task<Post?> GetByIdAsync(int id);

    Task<Post> CreateAsync(string title, string content, string? summary, string author);

    Task<bool> UpdateAsync(int id, string title, string content, string? summary);

    Task<bool> DeleteAsync(int id);
}