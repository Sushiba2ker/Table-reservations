using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models;

public class RevenueData
{
    [Required]
    public required string Period { get; set; }
    public decimal Revenue { get; set; }
}