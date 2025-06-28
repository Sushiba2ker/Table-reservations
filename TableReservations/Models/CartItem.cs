using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models;

public class CartItem
{
    public int ProductId { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}