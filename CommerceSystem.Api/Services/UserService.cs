using CommerceSystem.Api.Data;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.Exceptions;
using CommerceSystem.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using CommerceSystem.Api.Repositories;

namespace CommerceSystem.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    // CreateUser
    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        var emailExists = await _userRepository
            .EmailExistsAsync(request.Email);

        if (emailExists)
        {
            throw new ArgumentException("Email already exists.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    // GetUserById
    public async Task<User?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new UserNotFoundException($"User id {id} was not found.");

        return user;
    }

    // GetUserOrders
    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        var userExists = await _userRepository.GetByIdAsync(userId);

        if (userExists == null)
        {
            throw new UserNotFoundException($"User id {userId} not found.");
        }

        return await _userRepository.GetOrdersByUserIdAsync(userId);

    }

    // UpdateUser (patch)
    public async Task<User> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new UserNotFoundException($"User id {id} not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);

            if (emailExists)
            {
                throw new ArgumentException("Email already exists.");
            }

            user.Email = request.Email;
        }

        await _userRepository.SaveChangesAsync();

        return user;
    }
}