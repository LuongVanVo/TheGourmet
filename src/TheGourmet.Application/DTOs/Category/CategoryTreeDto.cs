namespace TheGourmet.Application.DTOs.Category;

public class CategoryTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // List of subcategories
    public List<CategoryTreeDto> Children { get; set; } = new List<CategoryTreeDto>();
}