using TableReservations.Common.Results;
using TableReservations.DTOs;

namespace TableReservations.Services.Interfaces;

/// <summary>
/// Service interface for Product business logic
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get all products with pagination
    /// </summary>
    Task<Result<IEnumerable<ProductListDto>>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    
    /// <summary>
    /// Get product by ID
    /// </summary>
    Task<Result<ProductDto>> GetProductByIdAsync(int id);
    
    /// <summary>
    /// Get products by category
    /// </summary>
    Task<Result<IEnumerable<ProductListDto>>> GetProductsByCategoryAsync(int categoryId);
    
    /// <summary>
    /// Create new product
    /// </summary>
    Task<Result<ProductDto>> CreateProductAsync(CreateUpdateProductDto productDto);
    
    /// <summary>
    /// Update existing product
    /// </summary>
    Task<Result<ProductDto>> UpdateProductAsync(int id, CreateUpdateProductDto productDto);
    
    /// <summary>
    /// Delete product
    /// </summary>
    Task<Result> DeleteProductAsync(int id);
    
    /// <summary>
    /// Search products by name or description
    /// </summary>
    Task<Result<IEnumerable<ProductListDto>>> SearchProductsAsync(string searchTerm);
    
    /// <summary>
    /// Check if product exists
    /// </summary>
    Task<Result<bool>> ProductExistsAsync(int id);
} 