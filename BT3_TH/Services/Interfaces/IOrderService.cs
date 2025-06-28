using BT3_TH.Common.Results;
using BT3_TH.DTOs;

namespace BT3_TH.Services.Interfaces;

/// <summary>
/// Service interface for Order business logic
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Process checkout and create order
    /// </summary>
    Task<Result<OrderDto>> ProcessCheckoutAsync(CheckoutDto checkoutDto, string userId);
    
    /// <summary>
    /// Get order by ID
    /// </summary>
    Task<Result<OrderDto>> GetOrderByIdAsync(int id);
    
    /// <summary>
    /// Get orders by user ID
    /// </summary>
    Task<Result<IEnumerable<OrderListDto>>> GetOrdersByUserAsync(string userId);
    
    /// <summary>
    /// Get all orders (admin only)
    /// </summary>
    Task<Result<IEnumerable<OrderListDto>>> GetAllOrdersAsync(int page = 1, int pageSize = 10);
    
    /// <summary>
    /// Calculate order total
    /// </summary>
    Task<Result<decimal>> CalculateOrderTotalAsync(List<CartItemDto> items);
    
    /// <summary>
    /// Update order status
    /// </summary>
    Task<Result<OrderDto>> UpdateOrderStatusAsync(int orderId, string status);
    
    /// <summary>
    /// Get order statistics
    /// </summary>
    Task<Result<object>> GetOrderStatisticsAsync();
} 