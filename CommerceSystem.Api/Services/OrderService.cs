using CommerceSystem.Api.Data;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.DTOs;
using CommerceSystem.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using CommerceSystem.Api.Repositories;

namespace CommerceSystem.Api.Services;

public class OrderService : IOrderService
{
    //private readonly StoreDbContext _context;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    /*
    public OrderService(StoreDbContext context)
    {
        _context = context;
    }
    */
    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }


    // CreateOrder
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // Order should contain at least one item
        if (request.Items == null || !request.Items.Any())
        {
            throw new ArgumentException("Order must contain at least one item");
        }

        // Check if the user exists
        var userExists = await _userRepository.GetByIdAsync(request.UserId);


        if (userExists == null)
        {
            throw new UserNotFoundException($"User id {request.UserId} was not found.");
        }


        var order = new Order
        {
            UserId = request.UserId,
            ShippingAddress = request.ShippingAddress,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        decimal total = 0;

        // Check item Ids against db
        foreach (var item in request.Items)
        {
            //var product = await _context.Products.FindAsync(item.ProductId);
            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product == null)
                throw new ProductNotFoundException($"Product id {item.ProductId} was not found.");

            if (product.StockQuantity < item.Quantity)
                throw new InsufficientStockException($"Not enough stock for {product.Name}.");

            if (item.Quantity < 1)
                throw new ArgumentException($"Invalid order quantity for id {item.ProductId}.");

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };


            // Calculate order total
            total += product.Price * item.Quantity;

            product.StockQuantity -= item.Quantity;

            order.Items.Add(orderItem);
        }

        order.Total = total;

        //_context.Orders.Add(order);

        //await _context.SaveChangesAsync();

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return order;
        /*
        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
        */
    }

    // GetOrderById
    public async Task<Order> GetOrderByIdAsync(int Id)
    {
        /*
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        */
        var order = await _orderRepository.GetByIdAsync(Id);

        if (order == null)
            throw new OrderNotFoundException($"Order id {Id} not found");
        /*
        if (order.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view this order.");
        */
        return order;
        /*
        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
        */
    }

    // UpdateOrder
    public async Task<Order> UpdateOrderAsync(int id, UpdateOrderRequest request)
    {
        /*
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        */
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            throw new OrderNotFoundException($"Order {id} not found.");
        }

        if (request.Status.HasValue)
        {
            // If cancelling, restore stock
            if (request.Status == OrderStatus.Cancelled &&
                order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.Items)
                {
                    //var product = await _context.Products
                    //    .FindAsync(item.ProductId);
                    var product = await _productRepository
                        .GetByIdAsync(item.ProductId);

                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }
            }

            order.Status = request.Status.Value;
        }

        //await _context.SaveChangesAsync();
        await _orderRepository.SaveChangesAsync();

        return order;
    }
}