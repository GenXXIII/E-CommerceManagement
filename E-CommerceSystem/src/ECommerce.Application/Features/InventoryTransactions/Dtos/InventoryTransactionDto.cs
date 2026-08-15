using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.InventoryTransactions.Dtos;

public class InventoryTransactionDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public InventoryTransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
