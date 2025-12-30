namespace TheGourmet.Application.Features.Categories.Results;

public class CategoryResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}