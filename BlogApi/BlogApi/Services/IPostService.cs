using BlogApi.Models;

namespace BlogApi.Services;

/// <summary>
/// 文章服务接口
/// </summary>
public interface IPostService
{
    // IEnumerable<Post>：返回文章集合（只读）
    IEnumerable<Post> GetAll();

    Post? GetById(int id);

    Post Create(string title, string content, string? summary);

    bool Update(int id, string title, string content, string? summary);

    bool Delete(int id);
}