using Xunit;
using Moq;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.Repositories;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.DTOs;
using CommerceSystem.Api.Exceptions;

namespace CommerceSystem.Tests.Services;

public class OrderServiceTests
{
    [Fact]
    public async Task GetByIdAsync_OrderExists_ReturnsOrder()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        var expectedOrder = new Order
        {
            Id = 1,
            //UserId = 10,
            Items = new List<OrderItem>()
        };

        orderRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expectedOrder);

        var service = new OrderService(
            orderRepo.Object,
            productRepo.Object,
            userRepo.Object
        );

        // Act
        var result = await service.GetOrderByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    /*
    [Fact]
    public async Task GetByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        var order = new Order
        {
            Id = 1,
            UserId = 10,
            Items = new List<OrderItem>()
        };

        orderRepo
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(order);

        var service = new OrderService(
            orderRepo.Object,
            productRepo.Object,
            userRepo.Object
        );

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetOrderByIdAsync(1, 1));
    }
    */

    [Fact]
    public async Task CreateOrderAsync_ValidRequest_CreatesOrderCorrectly()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        // User exists
        userRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1 });

        // Product exists with stock
        productRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Price = 10m,
                StockQuantity = 5
            });

        Order? savedOrder = null;

        orderRepo
            .Setup(x => x.AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => savedOrder = o)
            .Returns(Task.CompletedTask);

        var service = new OrderService(
            orderRepo.Object,
            productRepo.Object,
            userRepo.Object
        );

        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        var result = await service.CreateOrderAsync(request);

        Assert.NotNull(result);
        Assert.Equal(20m, result.Total); // 10 * 2
        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].ProductId);
        Assert.Equal(2, result.Items[0].Quantity);
        Assert.NotNull(savedOrder);
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ThrowsException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        userRepo
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1 });

        productRepo
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Price = 10m,
                StockQuantity = 1
            });

        var service = new OrderService(
            orderRepo.Object,
            productRepo.Object,
            userRepo.Object
        );

        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>
        {
            new CreateOrderItemRequest
            {
                ProductId = 1,
                Quantity = 5
            }
        }
        };

        await Assert.ThrowsAsync<InsufficientStockException>(() =>
            service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_UserDoesNotExist_ThrowsException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        userRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((User?)null);

        var service = new OrderService(orderRepo.Object, productRepo.Object, userRepo.Object);

        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 1 }
        }
        };

        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_ProductDoesNotExist_ThrowsException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        userRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1 });

        productRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Product?)null);

        var service = new OrderService(orderRepo.Object, productRepo.Object, userRepo.Object);

        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 1 }
        }
        };

        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_NoItems_ThrowsException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();

        var service = new OrderService(orderRepo.Object, productRepo.Object, userRepo.Object);

        userRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1 });

        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>()
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrderAsync_ExactStock_AllowsOrder()
    {
        var product = new Product
        {
            Id = 1,
            StockQuantity = 5,
            Price = 10m
        };

        var productRepo = new Mock<IProductRepository>();
        var userRepo = new Mock<IUserRepository>();
        var orderRepo = new Mock<IOrderRepository>();

        userRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1 });

        productRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        var service = new OrderService(orderRepo.Object, productRepo.Object, userRepo.Object);

        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 5 }
        }
        };

        var result = await service.CreateOrderAsync(request);

        Assert.NotNull(result);
    }
}
