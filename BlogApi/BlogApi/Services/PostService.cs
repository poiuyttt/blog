using BlogApi.Models;

namespace BlogApi.Services;

public class PostService : IPostService
{
    private static List<Post> _posts = new();
    private readonly ILogger<PostService> _logger;

    public PostService(ILogger<PostService> logger)
    {
        _logger = logger;
        if (_posts.Count == 0)
        {
            _posts.Add(new Post
            {
                Id = 1,
                Title = "第一篇文章",
                Content = "这是第一篇文章的内容",
                Summary = "这是第一篇文章的摘要",
                Author = "当前用户",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
            _posts.Add(new Post
            {
                Id = 2,
                Title = "第二篇文章",
                Content = "这是第二篇文章的内容",
                Summary = "这是第二篇文章的摘要",
                Author = "当前用户",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }
    }

    public IEnumerable<Post> GetAll() => _posts.OrderByDescending(p => p.CreatedAt);


    public Post? GetById(int id) => _posts.FirstOrDefault(p => p.Id == id);

    public Post Create(string title, string content, string? summary)
    {
        var post = new Post
        {
            Id = _posts.Count > 0 ? _posts.Max(p => p.Id) + 1 : 1,
            Title = title,
            Content = content,
            Summary = summary,
            Author = "当前用户",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _posts.Add(post);
        _logger.LogInformation($"创建文章Id:{post.Id}-Title:{post.Title}成功");
        return post;
    }

    public bool Update(int id, string title, string content, string? summary)
    {
        var post = _posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            _logger.LogWarning($"文章Id:{id}不存在");
            return false;
        }

        post.Title = title;
        post.Content = content;
        post.Summary = summary;
        post.UpdatedAt = DateTime.Now;
        _logger.LogInformation($"更新文章Id:{id}成功");
        return true;
    }

    public bool Delete(int id)
    {
        var post = _posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            _logger.LogWarning($"文章Id:{id}不存在");
            return false;
        }

        _posts.Remove(post);
        _logger.LogInformation($"删除文章Id:{id}成功");
        return true;
    }
}