using BlogApi.Models;

namespace BlogApi.Services
{
    /// <summary>
    /// 评论服务接口
    /// </summary>
    public interface ICommentService
    {

        Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);

        Task<Comment?> CreateAsync(int postId, string content, string author);

        Task<bool> DeleteAsync(int id);
    }
}
