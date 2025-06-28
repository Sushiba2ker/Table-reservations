using TableReservations.Common.Results;
using TableReservations.DTOs;
using TableReservations.Models;
using TableReservations.Repositories;
using TableReservations.Services.Interfaces;

namespace TableReservations.Services;

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



    // GetAllOrdersAsync with pagination - fix return type to OrderListDto
    public async Task<Result<IEnumerable<OrderListDto>>> GetAllOrdersAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
            {
                return Result<IEnumerable<OrderListDto>>.Failure("Page and pageSize must be positive integers", 400);
            }

            var allOrders = await _orderRepository.GetAllAsync();
            var paginatedOrders = allOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            var orderDtos = paginatedOrders.Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                ShippingAddress = o.ShippingAddress,
                ItemCount = o.OrderDetails.Count
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
                UserId = order.UserId,
                UserName = order.ApplicationUser?.UserName,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Error retrieving order: {ex.Message}", 500);
        }
    }

    public async Task<Result<OrderDto>> CreateOrderAsync(string userId, CheckoutDto checkoutDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<OrderDto>.Failure("User ID cannot be empty", 400);
            }

            if (!checkoutDto.Items.Any())
            {
                return Result<OrderDto>.Failure("Order must contain at least one item", 400);
            }

            // Calculate total price from cart items
            decimal totalPrice = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in checkoutDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return Result<OrderDto>.Failure($"Product with ID {item.ProductId} not found", 400);
                }

                if (item.Quantity <= 0)
                {
                    return Result<OrderDto>.Failure("Quantity must be positive", 400);
                }

                var lineTotal = product.Price * item.Quantity;
                totalPrice += lineTotal;

                orderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = lineTotal // Store line total in Price field
                });
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                ShippingAddress = checkoutDto.ShippingAddress,
                Notes = checkoutDto.Notes,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);

            var createdOrderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = null, // Will be populated if navigation property is loaded
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = null, // Will be populated if navigation property is loaded
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return Result<OrderDto>.Success(createdOrderDto, 201);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Error creating order: {ex.Message}", 500);
        }
    }

    public async Task<Result<OrderDto>> UpdateOrderAsync(int id, CheckoutDto checkoutDto)
    {
        try
        {
            if (id <= 0)
            {
                return Result<OrderDto>.Failure("Invalid order ID", 400);
            }

            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return Result<OrderDto>.NotFound($"Order with ID {id} not found");
            }

            // Recalculate total price
            decimal totalPrice = 0;
            var newOrderDetails = new List<OrderDetail>();

            foreach (var item in checkoutDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return Result<OrderDto>.Failure($"Product with ID {item.ProductId} not found", 400);
                }

                var lineTotal = product.Price * item.Quantity;
                totalPrice += lineTotal;

                newOrderDetails.Add(new OrderDetail
                {
                    OrderId = id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = lineTotal
                });
            }

            // Update order properties
            existingOrder.TotalPrice = totalPrice;
            existingOrder.ShippingAddress = checkoutDto.ShippingAddress;
            existingOrder.Notes = checkoutDto.Notes;
            existingOrder.OrderDetails = newOrderDetails;

            await _orderRepository.UpdateAsync(existingOrder);

            var updatedOrderDto = new OrderDto
            {
                Id = existingOrder.Id,
                UserId = existingOrder.UserId,
                UserName = existingOrder.ApplicationUser?.UserName,
                OrderDate = existingOrder.OrderDate,
                TotalPrice = existingOrder.TotalPrice,
                ShippingAddress = existingOrder.ShippingAddress,
                Notes = existingOrder.Notes,
                OrderDetails = existingOrder.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return Result<OrderDto>.Success(updatedOrderDto);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Error updating order: {ex.Message}", 500);
        }
    }

    public async Task<Result<OrderDto>> UpdateOrderStatusAsync(int id, string status)
    {
        try
        {
            // Note: Order model doesn't have Status field, this method may need to be removed
            // or we need to add Status to the Order model
            return Result<OrderDto>.Failure("Order status update not supported - no Status field in model", 400);
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

            await _orderRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting order: {ex.Message}", 500);
        }
    }



    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return Result<IEnumerable<OrderDto>>.Failure("Start date cannot be later than end date", 400);
            }

            var orders = await _orderRepository.GetAllAsync();
            var filteredOrders = orders.Where(o => 
                o.OrderDate.Date >= startDate.Date && 
                o.OrderDate.Date <= endDate.Date);
            
            var orderDtos = filteredOrders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserName = o.ApplicationUser?.UserName,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                ShippingAddress = o.ShippingAddress,
                Notes = o.Notes,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    ProductName = od.Product?.Name,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            }).OrderByDescending(o => o.OrderDate).ToList();

            return Result<IEnumerable<OrderDto>>.Success(orderDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderDto>>.Failure($"Error retrieving orders by date range: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersByStatusAsync(string status)
    {
        try
        {
            // Note: Order model doesn't have Status field
            // This method might not be applicable or we need to add Status to model
            return Result<IEnumerable<OrderDto>>.Failure("Order status filtering not supported - no Status field in model", 400);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderDto>>.Failure($"Error retrieving orders by status: {ex.Message}", 500);
        }
    }

    // GetOrderStatisticsAsync
    public async Task<Result<object>> GetOrderStatisticsAsync()
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();
            
            var statistics = new
            {
                TotalOrders = orders.Count(),
                TodayOrders = orders.Count(o => o.OrderDate.Date == DateTime.Today),
                ThisWeekOrders = orders.Count(o => o.OrderDate.Date >= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) && 
                                                   o.OrderDate.Date < DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek)),
                ThisMonthOrders = orders.Count(o => o.OrderDate.Month == DateTime.Now.Month && 
                                                    o.OrderDate.Year == DateTime.Now.Year),
                TotalRevenue = orders.Sum(o => o.TotalPrice),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalPrice) : 0,
                TopCustomers = orders.GroupBy(o => o.UserId)
                                    .Select(g => new { UserId = g.Key, OrderCount = g.Count(), TotalSpent = g.Sum(o => o.TotalPrice) })
                                    .OrderByDescending(x => x.TotalSpent)
                                    .Take(5)
                                    .ToList()
            };

            return Result<object>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Error retrieving order statistics: {ex.Message}", 500);
        }
    }

    // Add ProcessCheckoutAsync method as required by interface
    public async Task<Result<OrderDto>> ProcessCheckoutAsync(CheckoutDto checkoutDto, string userId)
    {
        // Delegate to CreateOrderAsync with parameters in correct order
        return await CreateOrderAsync(userId, checkoutDto);
    }

    // Add CalculateOrderTotalAsync method as required by interface
    public async Task<Result<decimal>> CalculateOrderTotalAsync(List<CartItemDto> items)
    {
        try
        {
            if (items == null || !items.Any())
            {
                return Result<decimal>.Success(0);
            }

            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return Result<decimal>.Failure($"Product with ID {item.ProductId} not found", 400);
                }

                if (item.Quantity <= 0)
                {
                    return Result<decimal>.Failure("Quantity must be positive", 400);
                }

                totalAmount += product.Price * item.Quantity;
            }

            return Result<decimal>.Success(totalAmount);
        }
        catch (Exception ex)
        {
            return Result<decimal>.Failure($"Error calculating order total: {ex.Message}", 500);
        }
    }

    // Add GetOrdersByUserAsync method as required by interface
    public async Task<Result<IEnumerable<OrderListDto>>> GetOrdersByUserAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<IEnumerable<OrderListDto>>.Failure("User ID cannot be empty", 400);
            }

            var orders = await _orderRepository.GetAllAsync();
            var userOrders = orders.Where(o => o.UserId == userId);
            
            var orderDtos = userOrders.Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                ShippingAddress = o.ShippingAddress,
                ItemCount = o.OrderDetails.Count
            }).OrderByDescending(o => o.OrderDate).ToList();

            return Result<IEnumerable<OrderListDto>>.Success(orderDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderListDto>>.Failure($"Error retrieving user orders: {ex.Message}", 500);
        }
    }
} 