using Microsoft.AspNetCore.Mvc;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.DTOs;
using CommerceSystem.Api.Exceptions;

namespace CommerceSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // CreateOrder
    [HttpPost]
    public async Task<ActionResult<Order>> Create(CreateOrderRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(request);

            var dto = new OrderDto
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

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                dto
            ); // 201

            /*
            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    userId = order.UserId,
                    orderId = order.Id
                },
            dto
            ); // 201
            */
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(ex.Message); // 400
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(ex.Message); // 404  
        }
    }

    // GetOrderById
    [HttpGet("{id}")]
    //[HttpGet("/api/users/{userId}/orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            var dto = new OrderDto
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

            return Ok(dto); // 200

            //return Ok(order); // 200
        }
        catch (OrderNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
        /*
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message); // 403
        }
        */

    }

    // PatchOrderById (OrderStatus)
    [HttpPatch("{id}")]
    public async Task<ActionResult<Order>> Update(int id, UpdateOrderRequest request)
    {
        try
        {
            var order = await _orderService.UpdateOrderAsync(id, request);

            var dto = new OrderDto
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

            return Ok(dto); // 200
        }
        catch (OrderNotFoundException ex)
        {
            return NotFound(ex.Message); // 404
        }
    }
}