using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models;

public class ProductImage
{
    public int Id { get; set; }
    
    [Required]
    public required string Url { get; set; }
    
    public int ProductId { get; set; }
    
    public Product? Product { get; set; }
}

