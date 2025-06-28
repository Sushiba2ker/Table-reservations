using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BT3_TH.Models;

public class Order
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public required string ShippingAddress { get; set; }
    public string? Notes { get; set; }
    [ForeignKey("UserId")]
    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new();
}