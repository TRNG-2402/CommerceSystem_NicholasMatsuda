using System.ComponentModel.DataAnnotations;
namespace CommerceSystem.Api.DTOs;

public class CreateOrderRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(200)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}