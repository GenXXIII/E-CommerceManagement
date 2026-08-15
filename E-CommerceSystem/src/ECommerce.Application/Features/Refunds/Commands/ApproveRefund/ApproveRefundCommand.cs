using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Features.Refunds.Dtos;
using ECommerce.Domain.Abstractions;
using MediatR;

namespace ECommerce.Application.Features.Refunds.Commands.ApproveRefund;

public class ApproveRefundCommand : IRequest<Result<RefundDto>>
{
    public Guid RefundId { get; set; }
}
