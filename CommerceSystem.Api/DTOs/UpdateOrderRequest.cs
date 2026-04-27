using CommerceSystem.Api.Models;
namespace CommerceSystem.Api.DTOs;

public class UpdateOrderRequest
{
    public OrderStatus? Status { get; set; }
}