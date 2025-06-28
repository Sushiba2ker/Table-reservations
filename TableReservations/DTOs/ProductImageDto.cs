using System.ComponentModel.DataAnnotations;

namespace TableReservations.DTOs;

/// <summary>
/// Data Transfer Object for Product Image information
/// </summary>
public class ProductImageDto
{
    public int Id { get; set; }
    
    [Required]
    public required string Url { get; set; }
    
    public int ProductId { get; set; }
} 