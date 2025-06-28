using System.ComponentModel.DataAnnotations;

namespace TableReservations.DTOs;

/// <summary>
/// Data Transfer Object for Cart Item information
/// </summary>
public class CartItemDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    [Range(0.01, 99999999.00)]
    public decimal Price { get; set; }
    
    [Range(1, 999)]
    public int Quantity { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public decimal TotalPrice => Price * Quantity;
}

/// <summary>
/// DTO for updating cart item quantity
/// </summary>
public class UpdateCartItemDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Range(1, 999)]
    public int Quantity { get; set; }
} 