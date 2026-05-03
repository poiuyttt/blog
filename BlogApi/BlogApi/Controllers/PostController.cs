using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private static List<Post> _posts = new List<Post>()
    {
        new Post { Id = 2, Title = "第二篇文章", Content = "这是第二篇文章的内容", Author = "Admin", CreatedAt = DateTime.Now },
        new Post { Id = 3, Title = "第三篇文章", Content = "这是第三篇文章的内容", Author = "Admin", CreatedAt = DateTime.Now }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_posts);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var post = _posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            return NotFound(new { Message = $"ID 为 {id} 的文章不存在" });
        }

        return Ok(post);
    }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public DateTime CreatedAt { get; set; }
}