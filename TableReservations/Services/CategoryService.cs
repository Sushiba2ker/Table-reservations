using TableReservations.Common.Results;
using TableReservations.DTOs;
using TableReservations.Models;
using TableReservations.Repositories;
using TableReservations.Services.Interfaces;

namespace TableReservations.Services;

/// <summary>
/// Category service implementation containing business logic
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = 0
            }).ToList();

            return Result<IEnumerable<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CategoryDto>>.Failure($"Error retrieving categories: {ex.Message}", 500);
        }
    }

    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<CategoryDto>.Failure("Invalid category ID", 400);
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                return Result<CategoryDto>.NotFound($"Category with ID {id} not found");
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = 0
            };

            return Result<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Error retrieving category: {ex.Message}", 500);
        }
    }

    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateUpdateCategoryDto categoryDto)
    {
        try
        {
            var existingCategories = await _categoryRepository.GetAllAsync();
            if (existingCategories.Any(c => c.Name.Equals(categoryDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<CategoryDto>.Failure($"Category with name '{categoryDto.Name}' already exists", 400);
            }

            var category = new Category
            {
                Name = categoryDto.Name
            };

            await _categoryRepository.AddAsync(category);

            var createdCategoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = 0
            };

            return Result<CategoryDto>.Success(createdCategoryDto, 201);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Error creating category: {ex.Message}", 500);
        }
    }

    public async Task<Result<CategoryDto>> UpdateCategoryAsync(int id, CreateUpdateCategoryDto categoryDto)
    {
        try
        {
            if (id <= 0)
            {
                return Result<CategoryDto>.Failure("Invalid category ID", 400);
            }

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return Result<CategoryDto>.NotFound($"Category with ID {id} not found");
            }

            var existingCategories = await _categoryRepository.GetAllAsync();
            if (existingCategories.Any(c => c.Id != id && c.Name.Equals(categoryDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<CategoryDto>.Failure($"Category with name '{categoryDto.Name}' already exists", 400);
            }

            existingCategory.Name = categoryDto.Name;
            await _categoryRepository.UpdateAsync(existingCategory);

            var updatedCategoryDto = new CategoryDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                ProductCount = 0
            };

            return Result<CategoryDto>.Success(updatedCategoryDto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Error updating category: {ex.Message}", 500);
        }
    }

    public async Task<Result> DeleteCategoryAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("Invalid category ID", 400);
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return Result.NotFound($"Category with ID {id} not found");
            }

            var products = await _productRepository.GetAllAsync();
            var hasProducts = products.Any(p => p.CategoryId == id);
            
            if (hasProducts)
            {
                return Result.Failure("Cannot delete category that contains products. Please move or delete products first.", 400);
            }

            await _categoryRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting category: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> CategoryExistsAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<bool>.Success(false);
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            return Result<bool>.Success(category != null);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking category existence: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<CategoryDto>>> GetCategoriesWithProductCountAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = products.Count(p => p.CategoryId == c.Id)
            }).ToList();

            return Result<IEnumerable<CategoryDto>>.Success(categoryDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CategoryDto>>.Failure($"Error retrieving categories with product count: {ex.Message}", 500);
        }
    }
} 