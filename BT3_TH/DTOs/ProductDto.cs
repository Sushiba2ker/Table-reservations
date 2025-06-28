using System.ComponentModel.DataAnnotations;

namespace BT3_TH.DTOs;

/// <summary>
/// Data Transfer Object for Product information
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Range(0.01, 99999999.00)]
    public decimal Price { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public int CategoryId { get; set; }
    
    public string? CategoryName { get; set; }
    
    public List<ProductImageDto>? Images { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Simplified Product DTO for listing purposes
/// </summary>
public class ProductListDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? CategoryName { get; set; }
}

/// <summary>
/// DTO for creating/updating products
/// </summary>
public class CreateUpdateProductDto
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Range(0.01, 99999999.00)]
    public decimal Price { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
} 