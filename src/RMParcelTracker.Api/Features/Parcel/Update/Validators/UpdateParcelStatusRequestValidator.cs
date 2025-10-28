using FluentValidation;
using RMParcelTracker.Api.Features.Parcel.Register.Validators;

namespace RMParcelTracker.Api.Features.Parcel.Update.Validators;

public class UpdateParcelStatusRequestValidator : AbstractValidator<UpdateParcelStatusRequestWithBarCode>
{
    public UpdateParcelStatusRequestValidator()
    {
        RuleFor(s => s.BarCode).NotEmpty();
        RuleFor(s => s.BarCode).SetValidator(new BarcodeValidator());
        RuleFor(s => s.StatusUpdate).NotEmpty().WithMessage("NewStatus cannot be empty");
        RuleFor(s => s.StatusUpdate).SetValidator(new ParcelStatusValidator());
    }
}