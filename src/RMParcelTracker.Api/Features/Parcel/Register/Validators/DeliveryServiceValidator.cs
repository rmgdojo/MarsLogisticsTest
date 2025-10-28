using FluentValidation;
using RMParcelTracker.Api.Common.Models;

namespace RMParcelTracker.Api.Features.Parcel.Register.Validators;

public class DeliveryServiceValidator : AbstractValidator<string>
{
    public DeliveryServiceValidator()
    {
        RuleFor(deliveryService => deliveryService).Must(deliveryService =>
                Enum.TryParse<DeliveryServiceType>(deliveryService, true, out _))
            .WithMessage("DeliveryService should be of type Standard or Express");
    }
}