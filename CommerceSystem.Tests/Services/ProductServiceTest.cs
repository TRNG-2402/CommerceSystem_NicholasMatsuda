using Xunit;
using Moq;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.Repositories;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.DTOs;
using CommerceSystem.Api.Exceptions;

namespace CommerceSystem.Tests.Services;

public class ProductServiceTest
{
    [Fact]
    public async Task GetProductByIdAsync_ExistingProduct_ReturnsProduct()
    {
        var repo = new Mock<IProductRepository>();

        repo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 10m,
                StockQuantity = 5
            });

        var service = new ProductService(repo.Object);

        var result = await service.GetProductByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task UpdateProductAsync_ValidRequest_UpdatesProductFields()
    {
        var repo = new Mock<IProductRepository>();

        var product = new Product
        {
            Id = 1,
            Name = "Old",
            Price = 10m,
            StockQuantity = 5
        };

        repo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        var service = new ProductService(repo.Object);

        var request = new UpdateProductRequest
        {
            Name = "New",
            Price = 20m
        };

        var result = await service.UpdateProductAsync(1, request);

        Assert.Equal("New", product.Name);
        Assert.Equal(20m, product.Price);
    }
}