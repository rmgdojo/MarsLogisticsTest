using System.Globalization;
using FluentValidation;

namespace RMParcelTracker.Api.Features.Parcel.Register.Validators;

public class BarcodeValidator : AbstractValidator<string>
{
    public BarcodeValidator()
    {
        RuleFor(b => b).Cascade(CascadeMode.Stop).Length(25).WithMessage("Barcode must be 25 characters. Starting with RMARS followed by 19 digits and then a capital letter in set [A-Z]")
            .Must(b => b.StartsWith("RMARS", true, CultureInfo.InvariantCulture))
            .WithMessage("Barcode should start with RMARS")
            .Must(b =>
            {
                var substring = b.Substring(5);
                for (var index = 0; index < 19; ++index)
                    if (!char.IsAsciiDigit(substring[index]))
                        return false;

                return true;
            }).WithMessage("Barcode should be followed by 19 digits after RMARS")
            .Must(b =>
            {
                var lastCharacter = b[24];
                return char.IsAsciiLetterUpper(lastCharacter);
            }).WithMessage("Last character in barcode should be upper case English alphabet in [A-Z]");
    }
}