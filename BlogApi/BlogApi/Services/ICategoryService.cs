using BlogApi.Models.Dtos;

namespace BlogApi.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(string name);
    Task<bool> UpdateAsync(int id, string name);
    Task<bool> DeleteAsync(int id);
}
