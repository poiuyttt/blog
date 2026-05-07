using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private IPostService _postService;
    private ILogger<PostController> _logger;

    public PostController(IPostService postService, ILogger<PostController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("获取所有文章");
        var posrs = _postService.GetAll();
        return Ok(posrs);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var post = _postService.GetById(id);
        if (post == null)
        {
            return NotFound(new { message = $"文章Id:{id}不存在" });
        }

        return Ok(post);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePostDto createPostDto)
    {
        _logger.LogInformation($"创建文章:{createPostDto.Title}");
        var post = _postService.Create(createPostDto.Title, createPostDto.Content, createPostDto.Summary);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CreatePostDto updatePostDto)
    {
        _logger.LogInformation($"更新文章:{id}");
        var success = _postService.Update(id, updatePostDto.Title, updatePostDto.Content, updatePostDto.Summary);
        if (!success)
            return NotFound(new { message = $"文章Id:{id}不存在" });
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation($"删除文章:{id}");
        var success = _postService.Delete(id);
        if (!success)
            return NotFound(new { message = $"文章Id:{id}不存在" });
        return NoContent();
    }
}