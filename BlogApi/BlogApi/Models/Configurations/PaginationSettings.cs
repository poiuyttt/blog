namespace BlogApi.Models.Configurations;

public class PaginationSettings
{
    public int PageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}