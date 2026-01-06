namespace TheGourmet.Application.Common.Models;

public class ProductQueryParameters
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; } // "price_asc", "price_desc", "newest", "oldest"
}