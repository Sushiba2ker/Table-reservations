using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models;

public class OrderViewModel
{
    [Required]
    public required Order Order { get; set; }
    public decimal TotalPrice { get; set; }
}