using System.ComponentModel.DataAnnotations;

namespace BT3_TH.DTOs;

/// <summary>
/// Data Transfer Object for Order information
/// </summary>
public class OrderDto
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public required string ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}

/// <summary>
/// DTO for creating orders (checkout process)
/// </summary>
public class CheckoutDto
{
    [Required]
    public required string ShippingAddress { get; set; }
    
    public string? Notes { get; set; }
    
    [Required]
    public List<CartItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for order details
/// </summary>
public class OrderDetailDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for order summary/listing
/// </summary>
public class OrderListDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? ShippingAddress { get; set; }
    public int ItemCount { get; set; }
} 