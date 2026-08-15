using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
namespace ECommerce.Application.Features.Addresses.Dtos;

public class AddressDto
{
    public Guid Id { get; set; }
    public Guid CustomerProfileId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Commune { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
