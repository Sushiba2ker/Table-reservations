using System.ComponentModel.DataAnnotations;

namespace BT3_TH.Models;

public class OrderViewModel
{
    [Required]
    public required Order Order { get; set; }
    public decimal TotalPrice { get; set; }
}