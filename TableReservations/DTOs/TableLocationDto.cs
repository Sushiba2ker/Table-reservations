using System.ComponentModel.DataAnnotations;

namespace TableReservations.DTOs;

/// <summary>
/// Data Transfer Object for Table Location information
/// </summary>
public class TableLocationDto
{
    public int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public bool IsAvailable { get; set; } = true;
}

/// <summary>
/// DTO for creating/updating table locations
/// </summary>
public class CreateUpdateTableLocationDto
{
    [Required]
    public required string Name { get; set; }
    
    public string? ImageUrl { get; set; }
} 