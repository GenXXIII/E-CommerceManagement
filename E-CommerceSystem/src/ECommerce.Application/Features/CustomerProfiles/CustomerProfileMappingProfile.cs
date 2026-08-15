using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;

using ECommerce.Application.Features.CustomerProfiles.Dtos;
using ECommerce.Domain.Entities;
using Mapster;

namespace ECommerce.Application.Features.CustomerProfiles;

public class CustomerProfileMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerProfile, CustomerProfileDto>();
    }
}
