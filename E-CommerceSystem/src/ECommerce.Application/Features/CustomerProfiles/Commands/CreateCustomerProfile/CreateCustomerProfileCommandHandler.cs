using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.CustomerProfiles.Commands.CreateCustomerProfile;

public class CreateCustomerProfileCommandHandler : IRequestHandler<CreateCustomerProfileCommand, Result<Guid>>
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerProfileCommandHandler(
        ICustomerProfileRepository customerProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _customerProfileRepository = customerProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateCustomerProfileCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customerProfile = new CustomerProfile(
                request.FirstName,
                request.LastName,
                new Email(request.Email),
                request.Phone);

            await _customerProfileRepository.AddAsync(customerProfile, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(customerProfile.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
