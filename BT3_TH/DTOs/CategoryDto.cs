using System.ComponentModel.DataAnnotations;

namespace BT3_TH.DTOs;

/// <summary>
/// Data Transfer Object for Category information
/// </summary>
public class CategoryDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    public int ProductCount { get; set; }
}

/// <summary>
/// DTO for creating/updating categories
/// </summary>
public class CreateUpdateCategoryDto
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
} 