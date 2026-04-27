using CommerceSystem.Api.Models;
using CommerceSystem.Api.DTOs;

namespace CommerceSystem.Api.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order> GetOrderByIdAsync(int orderId);
    Task<Order> UpdateOrderAsync(int id, UpdateOrderRequest request);
}