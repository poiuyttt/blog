using BlogApi.Models;
using BlogApi.Models.Dtos;

namespace BlogApi.Services;

/// <summary>
/// 文章服务接口
/// </summary>
public interface IPostService
{
    // IEnumerable<PostListDto>：返回文章列表 DTO 集合（只读）
    Task<IEnumerable<PostListDto>> GetAllAsync();

    Task<Post?> GetByIdAsync(int id);

    Task<Post> CreateAsync(string title, string content, string? summary, string author);

    Task<bool> UpdateAsync(int id, string title, string content, string? summary);

    Task<bool> DeleteAsync(int id);

    Task<(IEnumerable<PostListDto> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);

    Task<(IEnumerable<PostListDto> Data, int TotalCount)> SearchAsync(string keyword, int page, int pageSize);
}
