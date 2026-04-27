using CommerceSystem.Api.Models;

namespace CommerceSystem.Api.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task AddAsync(Order order);
    Task SaveChangesAsync();
}