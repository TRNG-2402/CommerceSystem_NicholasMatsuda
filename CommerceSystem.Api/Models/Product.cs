using System.ComponentModel.DataAnnotations;
namespace CommerceSystem.Api.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
}