using BlogApi.Models;
using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _postService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<PostListDto>>.Ok(posts, "获取文章列表成功"));
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
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
    {
        var post = await _postService.CreateAsync(
            createPostDto.Title,
            createPostDto.Content,
            createPostDto.Summary,
            "当前用户",
            createPostDto.CategoryId
        );
        return CreatedAtAction(
            nameof(GetById),
            new { id = post.Id },
            ApiResponse<Post>.Ok(post, "创建文章成功")
        );
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePostDto updatePostDto)
    {
        var success = await _postService.UpdateAsync(
            id,
            updatePostDto.Title,
            updatePostDto.Content,
            updatePostDto.Summary,
            updatePostDto.CategoryId
        );
        if (!success)
            return NotFound(ApiResponse<object>.NotFound($"文章Id:{id}不存在"));

        var post = await _postService.GetByIdAsync(id);
        return Ok(ApiResponse<Post>.Ok(post, "文章更新成功"));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _postService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.NotFound($"文章Id:{id}不存在"));
        return NoContent();
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5
    )
    {
        var (data, totalCount) = await _postService.GetPagedAsync(page, pageSize);
        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    Data = data,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                },
                "获取分页文章成功"
            )
        );
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string keyword = "",
        [FromQuery] int? categoryId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var (data, totalCount) = await _postService.SearchAsync(keyword, categoryId, page, pageSize);
        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    Data = data,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                },
                "搜索成功"
            )
        );
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var (data, totalCount) = await _postService.GetByCategoryAsync(categoryId, page, pageSize);
        return Ok(
                ApiResponse<object>.Ok(
                    new
                    {
                        Data = data,
                        TotalCount = totalCount,
                        Page = page,
                        PageSize = pageSize,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    },
                    "获取分类文章成功"
                )
            );
    }
}
