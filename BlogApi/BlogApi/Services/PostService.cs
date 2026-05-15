using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class PostService : IPostService
{
    private AppDbContext _context;
    private ILogger<PostService> _logger;

    public PostService(ILogger<PostService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IEnumerable<Post>> GetAllAsync() =>
        await _context.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();

    public async Task<Post?> GetByIdAsync(int id) =>
        await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Post> CreateAsync(string title, string content, string? summary, string author)
    {
        var post = new Post
        {

            Title = title,
            Content = content,
            Summary = summary,
            Author = author,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"创建文章Id:{post.Id}-Title:{post.Title}成功");
        return post;
    }

    public async Task<bool> UpdateAsync(int id, string title, string content, string? summary)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post == null)
        {
            _logger.LogWarning($"文章Id:{id}不存在");
            return false;
        }

        post.Title = title;
        post.Content = content;
        post.Summary = summary;
        post.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"更新文章Id:{id}成功");
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post == null)
        {
            _logger.LogWarning($"文章Id:{id}不存在");
            return false;
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"删除文章Id:{id}成功");
        return true;
    }
}
