using BT3_TH.Common.Results;
using BT3_TH.DTOs;
using BT3_TH.Models;
using BT3_TH.Repositories;
using BT3_TH.Services.Interfaces;

namespace BT3_TH.Services;

/// <summary>
/// Order service implementation containing business logic for order management
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<IEnumerable<OrderListDto>>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            
            var orderDtos = orders.Select(o => new OrderListDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                OrderDate = o.OrderDate,
                Status = o.Status,
                ItemCount = o.OrderDetails?.Count ?? 0
            }).ToList();

            return Result<IEnumerable<OrderListDto>>.Success(orderDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderListDto>>.Failure($"Error retrieving orders: {ex.Message}", 500);
        }
    }

    public async Task<Result<OrderDto>> GetOrderByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<OrderDto>.Failure("Invalid order ID", 400);
            }

            var order = await _orderRepository.GetByIdAsync(id);
            
            if (order == null)
            {
                return Result<OrderDto>.NotFound($"Order with ID {id} not found");
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                CustomerAddress = order.CustomerAddress,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Notes = order.Notes,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice
                }).ToList() ?? new List<OrderDetailDto>()
            };

            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Error retrieving order: {ex.Message}", 500);
        }
    }

    public async Task<Result<OrderDto>> CreateOrderAsync(CheckoutDto checkoutDto)
    {
        try
        {
            // Validate cart items
            if (checkoutDto.CartItems == null || !checkoutDto.CartItems.Any())
            {
                return Result<OrderDto>.Failure("Cart cannot be empty", 400);
            }

            // Calculate total and validate products
            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var cartItem in checkoutDto.CartItems)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    return Result<OrderDto>.Failure($"Product with ID {cartItem.ProductId} not found", 400);
                }

                if (cartItem.Quantity <= 0)
                {
                    return Result<OrderDto>.Failure("Quantity must be greater than 0", 400);
                }

                var unitPrice = product.Price;
                var totalPrice = unitPrice * cartItem.Quantity;
                totalAmount += totalPrice;

                orderDetails.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });
            }

            var order = new Order
            {
                CustomerName = checkoutDto.CustomerName,
                CustomerEmail = checkoutDto.CustomerEmail,
                CustomerPhone = checkoutDto.CustomerPhone,
                CustomerAddress = checkoutDto.CustomerAddress,
                TotalAmount = totalAmount,
                OrderDate = DateTime.Now,
                Status = "Pending",
                Notes = checkoutDto.Notes,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);

            var createdOrderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                CustomerAddress = order.CustomerAddress,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Notes = order.Notes,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice
                }).ToList()
            };

            return Result<OrderDto>.Success(createdOrderDto, 201);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Error creating order: {ex.Message}", 500);
        }
    }

    public async Task<Result<OrderDto>> UpdateOrderStatusAsync(int id, string status)
    {
        try
        {
            if (id <= 0)
            {
                return Result<OrderDto>.Failure("Invalid order ID", 400);
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return Result<OrderDto>.Failure("Status cannot be empty", 400);
            }

            var validStatuses = new[] { "Pending", "Confirmed", "Preparing", "Ready", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                return Result<OrderDto>.Failure($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}", 400);
            }

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return Result<OrderDto>.NotFound($"Order with ID {id} not found");
            }

            order.Status = status;
            await _orderRepository.UpdateAsync(order);

            var updatedOrderDto = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                CustomerAddress = order.CustomerAddress,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Notes = order.Notes,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice
                }).ToList() ?? new List<OrderDetailDto>()
            };

            return Result<OrderDto>.Success(updatedOrderDto);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Error updating order status: {ex.Message}", 500);
        }
    }

    public async Task<Result> DeleteOrderAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("Invalid order ID", 400);
            }

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return Result.NotFound($"Order with ID {id} not found");
            }

            // Only allow deletion of pending or cancelled orders
            if (order.Status != "Pending" && order.Status != "Cancelled")
            {
                return Result.Failure("Only pending or cancelled orders can be deleted", 400);
            }

            await _orderRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting order: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<OrderListDto>>> GetOrdersByStatusAsync(string status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return Result<IEnumerable<OrderListDto>>.Failure("Status cannot be empty", 400);
            }

            var orders = await _orderRepository.GetAllAsync();
            var filteredOrders = orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            
            var orderDtos = filteredOrders.Select(o => new OrderListDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                OrderDate = o.OrderDate,
                Status = o.Status,
                ItemCount = o.OrderDetails?.Count ?? 0
            }).ToList();

            return Result<IEnumerable<OrderListDto>>.Success(orderDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderListDto>>.Failure($"Error retrieving orders by status: {ex.Message}", 500);
        }
    }

    public async Task<Result<decimal>> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            
            var filteredOrders = orders.Where(o => 
                o.Status == "Completed" &&
                (startDate == null || o.OrderDate >= startDate) &&
                (endDate == null || o.OrderDate <= endDate)
            );

            var totalRevenue = filteredOrders.Sum(o => o.TotalAmount);
            
            return Result<decimal>.Success(totalRevenue);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Failure($"Error calculating total revenue: {ex.Message}", 500);
        }
    }

    public async Task<Result<int>> GetOrderCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            
            var filteredOrders = orders.Where(o => 
                (startDate == null || o.OrderDate >= startDate) &&
                (endDate == null || o.OrderDate <= endDate)
            );

            var orderCount = filteredOrders.Count();
            
            return Result<int>.Success(orderCount);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error counting orders: {ex.Message}", 500);
        }
    }
} 