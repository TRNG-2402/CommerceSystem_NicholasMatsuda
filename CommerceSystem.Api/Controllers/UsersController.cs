using Microsoft.AspNetCore.Mvc;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.Exceptions;
using CommerceSystem.Api.DTOs;

namespace CommerceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // CreateUser
    [HttpPost]
    public async Task<ActionResult<User>> Create(CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id },
                user
            ); // 201
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
    }

    // GetUserById
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
    }

    // GetUserOrders
    [HttpGet("{id}/orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(int id)
    {
        try
        {
            var orders = await _userService.GetOrdersByUserIdAsync(id);

            var result = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                Total = o.Total,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });

            return Ok(result);
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
    }

    // UpdateUser (patch)
    [HttpPatch("{id}")]
    public async Task<ActionResult<User>> Update(int id, UpdateUserRequest request)
    {
        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, request);
            return Ok(updatedUser); // 200
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
    }
}