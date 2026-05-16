using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Models.Dtos;
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

    public async Task<IEnumerable<PostListDto>> GetAllAsync() =>
        await _context
            .Posts.OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostListDto
            {
                Id = p.Id,
                Title = p.Title,
                Summary = p.Summary,
                Author = p.Author,
                CreatedAt = p.CreatedAt,
                CommentCount = p.Comments.Count(),
            })
            .ToListAsync();

    public async Task<Post?> GetByIdAsync(int id) =>
        await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Post> CreateAsync(
        string title,
        string content,
        string? summary,
        string author
    )
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
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"创建文章Id:{post.Id}-Title:{post.Title}成功");
        return post;
    }

    public async Task<bool> UpdateAsync(int id, string title, string content, string? summary)
    {
        var post = await _context.Posts.FindAsync(id);
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
        var post = await _context.Posts.FindAsync(id);
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

    public async Task<(IEnumerable<PostListDto> Data, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize
    )
    {
        var query = _context.Posts.OrderByDescending(p => p.CreatedAt);
        int totalCount = await query.CountAsync();

        var data = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostListDto
            {
                Id = p.Id,
                Title = p.Title,
                Summary = p.Summary,
                Author = p.Author,
                CreatedAt = p.CreatedAt,
                CommentCount = p.Comments.Count(),
            })
            .ToListAsync();
        return (data, totalCount);
    }
}
