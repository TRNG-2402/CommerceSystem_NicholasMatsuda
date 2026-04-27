namespace CommerceSystem.Api.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}