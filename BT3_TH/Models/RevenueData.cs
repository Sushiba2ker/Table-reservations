using System.ComponentModel.DataAnnotations;

namespace BT3_TH.Models;

public class RevenueData
{
    [Required]
    public required string Period { get; set; }
    public decimal Revenue { get; set; }
}