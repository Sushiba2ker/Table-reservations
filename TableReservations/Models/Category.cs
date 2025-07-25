using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models;

public class Category
{
    public int Id { get; set; }
    [Required, StringLength(100)]
    public required string Name { get; set; }
    public List<Product>? Products { get; set; }
}
