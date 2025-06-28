using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT3_TH.Models;

public class Product
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public required string Name { get; set; }
    [Range(0.01, 99999999.00)]
    public decimal Price { get; set; }
    public required string Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<ProductImage>? Images { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}

