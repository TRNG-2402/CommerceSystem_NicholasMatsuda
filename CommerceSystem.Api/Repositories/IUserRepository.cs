using CommerceSystem.Api.Models;

namespace CommerceSystem.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    Task SaveChangesAsync();
}