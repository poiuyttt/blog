using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "分类名不能为空")]
    public string Name { get; set; } = string.Empty;

    public int PostCount { get; set; }
}

public class CreateCategoryDto
{
    [Required(ErrorMessage = "分类名不能为空")]
    public string Name { get; set; } = string.Empty;
}
