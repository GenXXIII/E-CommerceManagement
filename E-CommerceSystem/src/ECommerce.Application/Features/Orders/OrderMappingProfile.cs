using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Domain.Entities;
using Mapster;

namespace ECommerce.Application.Features.Orders;

public class OrderMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OrderItem, OrderItemDto>();
        config.NewConfig<Order, OrderDto>();
    }
}
