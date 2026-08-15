using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
namespace ECommerce.Application.Features.ShoppingCarts.Dtos;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ShoppingCartDto
{
    public Guid Id { get; set; }
    public Guid CustomerProfileId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
