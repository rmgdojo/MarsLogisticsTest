using RMParcelTracker.Api.Common.Models;
using RMParcelTracker.Api.Common.Repository;
using RMParcelTracker.Api.Features.Parcel.Update;

namespace RMParcelTracker.Tests;

public class UpdateParcelStatusTests
{
    private readonly TestClock _clock;
    private readonly ParcelRepository _parcelRepository;
    private readonly UpdateParcelStatus _updateParcelStatus;

    public UpdateParcelStatusTests()
    {
        _parcelRepository = new ParcelRepository();
        _clock = new TestClock(new DateTime(2025, 10, 26));
        _updateParcelStatus = new UpdateParcelStatus(_parcelRepository, _clock);
    }

    [Fact]
    public void If_Parcel_Not_Found_Return_Failure()
    {
        var result =
            _updateParcelStatus.Handle(
                new UpdateParcelStatusRequestWithBarCode("RMARS1234567891234567891B", "Created"));
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData("Lost", 1)]
    [InlineData("LandedOnMars", 2)]
    [InlineData("OutForMartianDelivery", 3)]
    [InlineData("Delivered", 4)]
    [InlineData("Created", 5)]
    public void If_Invalid_Transition_From_Created_Throws_ArgumentException(string to, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        Assert.Throws<ArgumentException>(() => parcel.ChangeStatus(to, _clock.GetCurrentDate()));
    }

    [Fact]
    public void If_Transitioning_From_Created_To_OnRocketToMars_Happen_Before_LaunchDate_Fail_Transition()
    {
        var barcode = "RMARS1234567891234567891B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        Assert.Throws<ArgumentException>(() => parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate()));
    }

    [Fact]
    public void If_Transitioning_From_Created_To_OnRocketToMars_Happen_After_LaunchDate_Allow_Transition()
    {
        var barcode = "RMARS1234567891234567891B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90)); //Advance three months
        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        Assert.Equal(ParcelStatus.OnRocketToMars, parcel.Status);
    }

    [Theory]
    [InlineData("OutForMartianDelivery", 1)]
    [InlineData("Delivered", 2)]
    [InlineData("Created", 3)]
    [InlineData("OnRocketToMars", 4)]
    public void If_Invalid_Transition_From_OnRocketToMars_Throws_ArgumentException(string to, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90));

        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        Assert.Throws<ArgumentException>(() => parcel.ChangeStatus(to, _clock.GetCurrentDate()));
    }

    [Theory]
    [InlineData("LandedOnMars", 1)]
    [InlineData("Lost", 2)]
    public void Permit_Transitioning_From_OnRocketToMars_To_Allowed_States(string allowedState, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90)); //Advance three months
        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(2));
        parcel.ChangeStatus(allowedState, _clock.GetCurrentDate());
        Assert.Equal(allowedState, parcel.Status.ToString());
    }

    [Theory]
    [InlineData("Lost", 1)]
    [InlineData("Delivered", 2)]
    [InlineData("Created", 3)]
    [InlineData("LandedOnMars", 4)]
    [InlineData("OnRocketToMars", 5)]
    public void If_Invalid_Transition_From_LandedOnMars_Throws_ArgumentException(string to, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90));

        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        parcel.ChangeStatus("LandedOnMars", _clock.GetCurrentDate());

        Assert.Throws<ArgumentException>(() => parcel.ChangeStatus(to, _clock.GetCurrentDate()));
    }

    [Fact]
    public void Permit_Transitioning_From_LandedOnMars_To_Out_For_Martian_Delivery_State()
    {
        var barcode = "RMARS1234567891234567891B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90)); //Advance three months
        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(2));
        parcel.ChangeStatus("LandedOnMars", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(2));
        parcel.ChangeStatus("OutForMartianDelivery", _clock.GetCurrentDate());
    }

    [Theory]
    [InlineData("Created", 1)]
    [InlineData("OnRocketToMars", 2)]
    [InlineData("LandedOnMars", 3)]
    [InlineData("OutForMartianDelivery", 4)]
    public void If_Invalid_Transition_From_OutForMartianDelivery_Throws_ArgumentException(string to, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90));

        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        parcel.ChangeStatus("LandedOnMars", _clock.GetCurrentDate());
        parcel.ChangeStatus("OutForMartianDelivery", _clock.GetCurrentDate());

        Assert.Throws<ArgumentException>(() => parcel.ChangeStatus(to, _clock.GetCurrentDate()));
    }

    [Theory]
    [InlineData("Delivered", 1)]
    [InlineData("Lost", 2)]
    public void Permit_Transitioning_From_OutForMartianDelivery_To_Allowed_States(string allowedState, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90)); //Advance three months
        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(2));
        parcel.ChangeStatus("LandedOnMars", _clock.GetCurrentDate());
        parcel.ChangeStatus("OutForMartianDelivery", _clock.GetCurrentDate());

        parcel.ChangeStatus(allowedState, _clock.GetCurrentDate());
        Assert.Equal(allowedState, parcel.Status.ToString());
    }

    [Theory]
    [InlineData("Lost", 1)]
    [InlineData("Delivered", 2)]
    public void TransitioningFrom_TerminalStates_Not_Allowed(string terminalState, int index)
    {
        var barcode = $"RMARS123456789123456789{index}B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90)); //Advance three months
        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        parcel.ChangeStatus("LandedOnMars", _clock.GetCurrentDate());
        parcel.ChangeStatus("OutForMartianDelivery", _clock.GetCurrentDate());
        parcel.ChangeStatus(terminalState, _clock.GetCurrentDate());

        Assert.Throws<ArgumentException>(() => parcel.ChangeStatus(terminalState, _clock.GetCurrentDate()));
    }

    [Fact]
    public void Parcel_Can_Get_Through_Allowed_States_And_History_Is_Populated()
    {
        var barcode = "RMARS1234567891234567891B";
        var parcel = new Parcel(barcode, "sender", "recipient", "express", "paper", _clock.GetCurrentDate());
        _parcelRepository.Add(parcel);
        _clock.Advance(TimeSpan.FromDays(90)); //Advance three months
        parcel.ChangeStatus("OnRocketToMars", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(1));
        parcel.ChangeStatus("LandedOnMars", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(2));
        parcel.ChangeStatus("OutForMartianDelivery", _clock.GetCurrentDate());
        _clock.Advance(TimeSpan.FromDays(3));
        parcel.ChangeStatus("Delivered", _clock.GetCurrentDate());

        var parcelHistory = parcel.History;

        ParcelAuditTrail[] expectedHistory = [
            new (ParcelStatus.Created,DateOnly.FromDateTime(new DateTime(2025,10,26))),
            new (ParcelStatus.OnRocketToMars,DateOnly.FromDateTime(new DateTime(2026,01,24))),
            new (ParcelStatus.LandedOnMars,DateOnly.FromDateTime(new DateTime(2026,01,25))),
            new (ParcelStatus.OutForMartianDelivery,DateOnly.FromDateTime(new DateTime(2026,01,27))),
            new (ParcelStatus.Delivered,DateOnly.FromDateTime(new DateTime(2026,01,30)))
        ];

        Assert.Equal(expectedHistory, parcelHistory);
    }
}