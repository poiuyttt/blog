namespace BlogApi.Models.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PostCount { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
}
