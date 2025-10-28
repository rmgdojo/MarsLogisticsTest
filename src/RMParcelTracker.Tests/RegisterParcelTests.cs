using RMParcelTracker.Api.Common.Models;
using RMParcelTracker.Api.Features.Parcel.Register;

namespace RMParcelTracker.Tests;

public class RegisterParcelTests
{
    private const string ValidBarcode = "RMARS1234567891234567891B";
    private const string ValidServiceType = "Express";
    private static readonly DateTime TestDate = new(2025, 10, 26);
    private readonly RegisterParcel _registerParcel = new(new TestClock(TestDate));

    [Fact]
    public void On_Registering_Parcel_Initial_Status_Should_Be_Created()
    {
        var result = _registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient",
            ValidServiceType, "other contents"));
        Assert.Equal(ParcelStatus.Created, result.Value.Status);
    }

    [Fact]
    public void On_Registering_Parcel_Origin_Should_Be_Starport_Thames_Estuary()
    {
        var result = _registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient",
            ValidServiceType, "other contents"));
        Assert.Equal("Starport Thames Estuary", result.Value.Origin);
    }

    [Fact]
    public void On_Registering_Parcel_Destination_Should_Be_New_London()
    {
        var result = _registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient",
            ValidServiceType, "other contents"));
        Assert.Equal("New London", result.Value.Destination);
    }

    [Theory]
    [InlineData("Standard", 180)]
    [InlineData("Express", 90)]
    public void On_Registering_Parcel_Eta_Is_Calculated_Based_On_DeliveryType(string deliveryType, int expectedEta)
    {
        var result =
            _registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient", deliveryType,
                "other contents"));
        Assert.Equal(expectedEta, result.Value.Delivery.Eta);
    }

    [Theory]
    [InlineData("Express", "2025-11-05")]
    [InlineData("Standard", "2027-12-01")]
    public void On_Registering_Parcel_LaunchDate_Is_Calculated_Based_On_DeliveryType(string deliveryType,
        string expectedLaunchDate)
    {
        var result =
            _registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient", deliveryType,
                "other contents"));
        Assert.Equal(DateOnly.Parse(expectedLaunchDate), result.Value.Delivery.LaunchDate);
    }
    
    [Fact]
    public void On_Registering_Parcel_LaunchDate_Is_Same_If_Today_Is_LaunchDate_For_Express_Delivery()
    {
        var firstWednesdayOfDecember = new DateTime(2030, 1, 02);
        var registerParcel = new RegisterParcel(new TestClock(firstWednesdayOfDecember));
        var result =
            registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient", "express",
                "other contents"));
        Assert.Equal(DateOnly.FromDateTime(firstWednesdayOfDecember), result.Value.Delivery.LaunchDate);
    }

    [Fact]
    public void On_Registering_Parcel_LaunchDate_Is_Same_If_Today_Is_LaunchDate_For_Standard_Delivery()
    {
        var launchDate = new DateTime(2025, 12, 03);
        var registerParcel = new RegisterParcel(new TestClock(launchDate));
        var result =
            registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient", "express",
                "other contents"));
        Assert.Equal(DateOnly.FromDateTime(launchDate), result.Value.Delivery.LaunchDate);
    }

    [Theory]
    [InlineData("Express", "2026-02-03")]
    [InlineData("Standard", "2028-05-29")]
    public void On_Registering_Parcel_EstimatedArrivalDate_Is_Calculated_Based_On_DeliveryType(string deliveryType,
        string expectedArrivalDate)
    {
        var result =
            _registerParcel.Handle(new RegisterParcelRequest(ValidBarcode, "sender", "recipient", deliveryType,
                "other contents"));
        Assert.Equal(DateOnly.Parse(expectedArrivalDate), result.Value.Delivery.EstimatedArrivalDate);
    }
}