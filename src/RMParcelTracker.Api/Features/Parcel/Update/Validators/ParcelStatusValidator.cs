using FluentValidation;
using RMParcelTracker.Api.Common.Models;

namespace RMParcelTracker.Api.Features.Parcel.Update.Validators
{
    public class ParcelStatusValidator : AbstractValidator<string>
    {
        public ParcelStatusValidator()
        {
            RuleFor(s => s).Must(s =>
            {
                var partStatusEnum = ParcelStatusConverter.GetParcelStatus(s);
                return partStatusEnum is not null;
            }).WithMessage("NewStatus is not valid");
        }
    }
}
