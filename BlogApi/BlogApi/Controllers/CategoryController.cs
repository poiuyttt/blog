using BlogApi.Models;
using BlogApi.Models.Dtos;
using BlogApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories, "获取分类列表成功"));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto.Name);
        return Ok(ApiResponse<CategoryDto>.Ok(category, "创建分类成功"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _categoryService.DeleteAsync(id);
        if (!success)
            return NotFound(ApiResponse<CategoryDto>.NotFound("分类不存在"));

        return NoContent();
    }
}
