using CommerceSystem.Api.Models;
using CommerceSystem.Api.Exceptions;
using CommerceSystem.Api.Repositories;
using CommerceSystem.Api.DTOs;


namespace CommerceSystem.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }


    // GetAllProducts
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    // GetProductById
    public async Task<Product> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            throw new ProductNotFoundException($"Product id {id} was not found.");

        return product;
    }

    // CreateProduct
    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Category = request.Category,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };


        // Check for valid name
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("Product name is required.");
        }

        // Negative price
        if (product.Price < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }

        // Check for duplicate SKU
        var exists = await _productRepository.SkuExistsAsync(product.SKU);

        if (exists)
        {
            throw new DuplicateSkuException(product.SKU);
        }

        //_context.Products.Add(product);
        //await _context.SaveChangesAsync();
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product;
    }


    // DeleteProduct
    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            throw new ProductNotFoundException($"Product id {id} not found.");

        await _productRepository.DeleteByIdAsync(id);
    }


    // UpdateProduct
    public async Task<Product> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            throw new ProductNotFoundException($"Product {id} not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            product.Name = request.Name;
        }

        if (request.Price.HasValue)
        {
            if (request.Price <= 0)
                throw new ArgumentException("Price must be greater than 0.");

            product.Price = request.Price.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            product.Description = request.Description;
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            product.Category = request.Category;
        }

        await _productRepository.SaveChangesAsync();

        return product;
    }
}