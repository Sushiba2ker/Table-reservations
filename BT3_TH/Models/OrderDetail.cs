using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BT3_TH.Models;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    [ForeignKey("OrderId")]
    [ValidateNever]
    public Order? Order { get; set; }
    
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product? Product { get; set; }
}
