using BlogApi.Models;
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
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("获取所有文章");
        var posrs = await _postService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<Post>>.Ok(posrs, "获取文章列表成功"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _postService.GetByIdAsync(id);
        if (post == null)
        {
            return NotFound(ApiResponse<Post>.NotFound($"文章Id:{id}不存在"));
        }

        return Ok(ApiResponse<Post>.Ok(post, "获取文章成功"));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
    {
        _logger.LogInformation($"创建文章:{createPostDto.Title}");
        var post = await _postService.CreateAsync(createPostDto.Title, createPostDto.Content, createPostDto.Summary, createPostDto.Author);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, ApiResponse<Post>.Ok(post, "创建文章成功"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePostDto updatePostDto)
    {
        _logger.LogInformation($"更新文章:{id}");
        var success = await _postService.UpdateAsync(id, updatePostDto.Title, updatePostDto.Content, updatePostDto.Summary);
        if (!success)
            return NotFound(ApiResponse<object>.NotFound($"文章Id:{id}不存在"));
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation($"删除文章:{id}");
        var success = await _postService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.NotFound($"文章Id:{id}不存在"));
        return NoContent();
    }
}