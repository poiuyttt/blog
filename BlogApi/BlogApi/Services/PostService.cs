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
                CategoryName = p.Category != null ? p.Category.Name : null,
            })
            .ToListAsync();

    public async Task<Post?> GetByIdAsync(int id) =>
        await _context.Posts.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Post> CreateAsync(
        string title,
        string content,
        string? summary,
        string author,
        int? categoryId = null
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
            CategoryId = categoryId,
        };
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"创建文章Id:{post.Id}-Title:{post.Title}成功");
        return post;
    }

    public async Task<bool> UpdateAsync(
        int id,
        string title,
        string content,
        string? summary,
        int? categoryId = null
    )
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
        post.CategoryId = categoryId;
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
                CategoryName = p.Category != null ? p.Category.Name : null,
            })
            .ToListAsync();
        return (data, totalCount);
    }

    public async Task<(IEnumerable<PostListDto> Data, int TotalCount)> SearchAsync(
        string keyword,
        int? categoryId,
        int page,
        int pageSize
    )
    {
        var query = _context.Posts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p =>
                p.Title.Contains(keyword)
                || (p.Summary != null && p.Summary.Contains(keyword))
                || p.Content.Contains(keyword)
            );
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        query = query.OrderByDescending(p => p.CreatedAt);
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
                CategoryName = p.Category != null ? p.Category.Name : null,
            })
            .ToListAsync();

        _logger.LogInformation($"搜索关键词:{keyword}, 找到:{totalCount}条结果");
        return (data, totalCount);
    }

    public async Task<(IEnumerable<PostListDto> Data, int TotalCount)> GetByCategoryAsync(
        int categoryId,
        int page,
        int pageSize
    )
    {
        var query = _context
            .Posts.Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt);
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
                CategoryName = p.Category != null ? p.Category.Name : null,
            })
            .ToListAsync();

        return (data, totalCount);
    }
}
