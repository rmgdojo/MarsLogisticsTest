using FluentValidation.TestHelper;
using RMParcelTracker.Api.Features.Parcel.Register;
using RMParcelTracker.Api.Features.Parcel.Register.Validators;

namespace RMParcelTracker.Tests;

public class RegisterParcelRequestValidatorTests
{
    private const string ValidBarcode = "RMARS1234567891234567891B";
    private const string ValidServiceType = "Express";
    private readonly RegisterParcelRequestValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Sender_Cannot_Be_Empty(string sender)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(ValidBarcode, sender, "recipient", ValidServiceType, "contents");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Sender);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Recipient_Cannot_Be_Empty(string recipient)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(ValidBarcode, "sender", recipient, ValidServiceType, "contents");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Recipient);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Content_Description_Cannot_Be_Empty(string description)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(ValidBarcode, "sender", "recipient", ValidServiceType, description);
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Contents);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Barcode_Cannot_Be_Empty(string barcode)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(barcode, "sender", "recipient", ValidServiceType, "description");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Barcode);
    }

    [Fact]
    public void BarCode_Should_Start_With_RMARS_String()
    {
        var registerParcelRequest =
            new RegisterParcelRequest("SMARS", "sender", "recipient", ValidServiceType, "description");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Barcode);
    }

    [Theory]
    [InlineData("RMARS12345678912345678B")] // 18 digits
    [InlineData("RMARS1234567891234")] // lesser digits
    [InlineData("RMARS")] // no digits
    [InlineData("RMARSs234567b91234")] // non digits from index 5
    [InlineData("RMARS൦൧൨൩൪൫൬൭൮൯൦൧൨൩൪൫൬൭൮൯൧൧B")] // 0-9 digits from other numeric system
    public void BarCode_Should_Be_FollowedBy_19_Digits_After_RMARS_String(string barcode)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(barcode, "sender", "recipient", ValidServiceType, "description");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Barcode);
    }

    [Theory]
    [InlineData("RMARS1234567891234567891")] // 20 digits
    [InlineData("RMARS1234567891234567891Κ")] //19 digits + non A-Z character
    public void BarCode_Should_End_With_One_Character_From_A_Z_Set(string barcode)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(barcode, "sender", "recipient", ValidServiceType, "description");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.Barcode);
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData("NonExpress")]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void DeliveryService_Should_Be_Valid(string deliveryService)
    {
        var registerParcelRequest =
            new RegisterParcelRequest(ValidBarcode, "sender", "recipient", deliveryService, "description");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldHaveValidationErrorFor(request => request.DeliveryService);
    }

    [Fact]
    public void ValidationSucceeds_If_Request_Is_Valid()
    {
        var registerParcelRequest =
            new RegisterParcelRequest(ValidBarcode, "sender", "recipient", ValidServiceType, "description");
        var validationResult = _validator.TestValidate(registerParcelRequest);
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}