using FluentValidation;

namespace RMParcelTracker.Api.Features.Parcel.Register.Validators;

public class RegisterParcelRequestValidator : AbstractValidator<RegisterParcelRequest>
{
    public RegisterParcelRequestValidator()
    {
        RuleFor(req => req.Sender).NotEmpty();
        RuleFor(req => req.Recipient).NotEmpty();
        RuleFor(req => req.Contents).NotEmpty();
        RuleFor(req => req.Barcode).NotEmpty().SetValidator(new BarcodeValidator());
        RuleFor(req => req.DeliveryService).NotEmpty().SetValidator(new DeliveryServiceValidator());
    }
}