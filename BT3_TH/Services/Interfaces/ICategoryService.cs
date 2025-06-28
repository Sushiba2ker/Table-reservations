using BT3_TH.Common.Results;
using BT3_TH.DTOs;

namespace BT3_TH.Services.Interfaces;

/// <summary>
/// Service interface for Category business logic
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get all categories
    /// </summary>
    Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<Result<CategoryDto>> GetCategoryByIdAsync(int id);
    
    /// <summary>
    /// Create new category
    /// </summary>
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateUpdateCategoryDto categoryDto);
    
    /// <summary>
    /// Update existing category
    /// </summary>
    Task<Result<CategoryDto>> UpdateCategoryAsync(int id, CreateUpdateCategoryDto categoryDto);
    
    /// <summary>
    /// Delete category
    /// </summary>
    Task<Result> DeleteCategoryAsync(int id);
    
    /// <summary>
    /// Check if category exists
    /// </summary>
    Task<Result<bool>> CategoryExistsAsync(int id);
    
    /// <summary>
    /// Get categories with product count
    /// </summary>
    Task<Result<IEnumerable<CategoryDto>>> GetCategoriesWithProductCountAsync();
} 