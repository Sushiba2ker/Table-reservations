using BT3_TH.Common.Results;
using BT3_TH.DTOs;
using BT3_TH.Models;
using BT3_TH.Repositories;
using BT3_TH.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BT3_TH.Services;

/// <summary>
/// Product service implementation containing business logic
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IEnumerable<ProductListDto>>> GetAllProductsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var products = await _productRepository.GetAllAsync();
            
            var productList = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category?.Name
                })
                .ToList();

            return Result<IEnumerable<ProductListDto>>.Success(productList);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProductListDto>>.Failure($"Error retrieving products: {ex.Message}", 500);
        }
    }

    public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<ProductDto>.Failure("Invalid product ID", 400);
            }

            var product = await _productRepository.GetByIdAsync(id);
            
            if (product == null)
            {
                return Result<ProductDto>.NotFound($"Product with ID {id} not found");
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Images = product.Images?.Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    Url = img.Url,
                    ProductId = img.ProductId
                }).ToList()
            };

            return Result<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Error retrieving product: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<ProductListDto>>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            if (categoryId <= 0)
            {
                return Result<IEnumerable<ProductListDto>>.Failure("Invalid category ID", 400);
            }

            // Check if category exists
            var categoryExists = await _categoryRepository.GetByIdAsync(categoryId);
            if (categoryExists == null)
            {
                return Result<IEnumerable<ProductListDto>>.NotFound($"Category with ID {categoryId} not found");
            }

            var products = await _productRepository.GetAllAsync();
            var categoryProducts = products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category?.Name
                })
                .ToList();

            return Result<IEnumerable<ProductListDto>>.Success(categoryProducts);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProductListDto>>.Failure($"Error retrieving products by category: {ex.Message}", 500);
        }
    }

    public async Task<Result<ProductDto>> CreateProductAsync(CreateUpdateProductDto productDto)
    {
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                return Result<ProductDto>.Failure($"Category with ID {productDto.CategoryId} not found", 400);
            }

            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageUrl = productDto.ImageUrl,
                CategoryId = productDto.CategoryId
            };

            await _productRepository.AddAsync(product);

            var createdProductDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = category.Name
            };

            return Result<ProductDto>.Success(createdProductDto, 201);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Error creating product: {ex.Message}", 500);
        }
    }

    public async Task<Result<ProductDto>> UpdateProductAsync(int id, CreateUpdateProductDto productDto)
    {
        try
        {
            if (id <= 0)
            {
                return Result<ProductDto>.Failure("Invalid product ID", 400);
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return Result<ProductDto>.NotFound($"Product with ID {id} not found");
            }

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                return Result<ProductDto>.Failure($"Category with ID {productDto.CategoryId} not found", 400);
            }

            // Update product properties
            existingProduct.Name = productDto.Name;
            existingProduct.Price = productDto.Price;
            existingProduct.Description = productDto.Description;
            existingProduct.CategoryId = productDto.CategoryId;
            
            if (!string.IsNullOrEmpty(productDto.ImageUrl))
            {
                existingProduct.ImageUrl = productDto.ImageUrl;
            }

            await _productRepository.UpdateAsync(existingProduct);

            var updatedProductDto = new ProductDto
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Price = existingProduct.Price,
                Description = existingProduct.Description,
                ImageUrl = existingProduct.ImageUrl,
                CategoryId = existingProduct.CategoryId,
                CategoryName = category.Name
            };

            return Result<ProductDto>.Success(updatedProductDto);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Error updating product: {ex.Message}", 500);
        }
    }

    public async Task<Result> DeleteProductAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("Invalid product ID", 400);
            }

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Result.NotFound($"Product with ID {id} not found");
            }

            await _productRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting product: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<ProductListDto>>> SearchProductsAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Result<IEnumerable<ProductListDto>>.Failure("Search term cannot be empty", 400);
            }

            var products = await _productRepository.GetAllAsync();
            var searchResults = products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category?.Name
                })
                .ToList();

            return Result<IEnumerable<ProductListDto>>.Success(searchResults);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProductListDto>>.Failure($"Error searching products: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> ProductExistsAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<bool>.Success(false);
            }

            var product = await _productRepository.GetByIdAsync(id);
            return Result<bool>.Success(product != null);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking product existence: {ex.Message}", 500);
        }
    }
} 