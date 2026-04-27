using CommerceSystem.Api.Models;
using CommerceSystem.Api.DTOs;

namespace CommerceSystem.Api.Services;

public interface IUserService
{
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<User?> GetByIdAsync(int id);
    Task<List<Order>> GetOrdersByUserIdAsync(int userId);
    Task<User> UpdateUserAsync(int id, UpdateUserRequest request);
}