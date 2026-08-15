

namespace ECommerce.Application.Features.CustomerProfiles.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, Result>
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerProfileCommandHandler(
        ICustomerProfileRepository customerProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _customerProfileRepository = customerProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateCustomerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var customerProfile = await _customerProfileRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customerProfile == null)
            return Result.Failure("Customer profile not found.");

        try
        {
            if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
                customerProfile.UpdateName(request.FirstName ?? customerProfile.FirstName, request.LastName ?? customerProfile.LastName);

            if (!string.IsNullOrWhiteSpace(request.Email))
                customerProfile.UpdateEmail(new Email(request.Email));

            if (request.Phone != null)
                customerProfile.UpdatePhone(request.Phone);

            _customerProfileRepository.Update(customerProfile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
