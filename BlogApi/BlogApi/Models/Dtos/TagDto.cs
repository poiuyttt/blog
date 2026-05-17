namespace BlogApi.Models.Dtos;

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PostCount { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
}
