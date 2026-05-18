using BlogApi.Data;
using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CommentService> _logger;

    public CommentService(ILogger<CommentService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(int postId)
    {
        var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            _logger.LogWarning($"获取评论失败：文章 {postId} 不存在");
            return Enumerable.Empty<Comment>();
        }

        return await _context
            .Comments.Where(p => p.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment?> CreateAsync(int postId, string content, string author, int userId)
    {
        var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            _logger.LogWarning($"创建评论失败：文章 {postId} 不存在");
            return null;
        }

        var comment = new Comment
        {
            PostId = postId,
            Content = content,
            Author = author,
            CreatedAt = DateTime.Now,
            UserId = userId,
        };
        await _context.Comments.AddAsync(comment);

        await _context.SaveChangesAsync();

        _logger.LogInformation($"评论创建成功：PostId = {postId},Id = {comment.Id}");

        return comment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            _logger.LogWarning($"删除失败：评论 {id} 不存在");
            return false;
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"评论删除成功:{id}");
        return true;
    }
}
