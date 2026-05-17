using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services;

public class CategoryService : ICategoryService
{
    private AppDbContext _context;
    private ILogger<CategoryService> _logger;

    public CategoryService(ILogger<CategoryService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync() =>
        await _context
            .Categories.OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                PostCount = c.Posts.Count,
            })
            .ToListAsync();

    public async Task<CategoryDto?> GetByIdAsync(int id) =>
        await _context
            .Categories.Where(c => c.Id == id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                PostCount = c.Posts.Count,
            })
            .FirstOrDefaultAsync();

    public async Task<CategoryDto> CreateAsync(string name)
    {
        var category = new Category { Name = name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"创建分类Id:{category.Id}-Name:{category.Name}成功");
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            PostCount = 0,
        };
    }

    public async Task<bool> UpdateAsync(int id, string name)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            _logger.LogWarning($"分类Id:{id}不存在");
            return false;
        }

        category.Name = name;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"更新分类Id:{id}成功");
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            _logger.LogWarning($"分类Id:{id}不存在");
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"删除分类Id:{id}成功");
        return true;
    }
}
