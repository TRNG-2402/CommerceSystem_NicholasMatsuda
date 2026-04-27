using Microsoft.AspNetCore.Mvc;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.Exceptions;
using CommerceSystem.Api.DTOs;

namespace CommerceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GetAll
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products); // 200
    }

    // GetById
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product); // 200    
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }

        //if (product == null)
        //    return NotFound(); // 404

        //return Ok(product); // 200    
    }

    // CreateProduct
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
    {
        try
        {
            var createdProduct = await _productService.CreateProductAsync(request);

            var dto = new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                SKU = createdProduct.SKU,
                Category = createdProduct.Category,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            /*
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                createdProduct
            ); // 201
            */
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
        catch (DuplicateSkuException ex)
        {
            return Conflict(ex.Message); // 409
        }
    }


    // Delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return NoContent(); // 204
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
    }

    // PatchProductById
    [HttpPatch("{id}")]
    public async Task<ActionResult<Product>> Update(int id, UpdateProductRequest request)
    {
        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, request);

            var dto = new ProductDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                SKU = updatedProduct.SKU,
                Category = updatedProduct.Category,
                Description = updatedProduct.Description,
                Price = updatedProduct.Price,
                StockQuantity = updatedProduct.StockQuantity
            };

            return Ok(dto); // 200
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
    }
}