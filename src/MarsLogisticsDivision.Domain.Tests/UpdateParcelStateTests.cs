using MarsLogisticsDivision.Domain.Entities;
using MarsLogisticsDivision.Domain.Enums;
using MarsLogisticsDivision.Domain.Exceptions;
using MarsLogisticsDivision.Domain.Factories;

namespace MarsLogisticsDivision.Domain.Tests;

public class UpdateParcelStateTests
{
    [Fact]
    public void UpdateState_AddsHistoryEntry()
    {
        // Arrange
        var parcel = new Parcel
        {
            Barcode = "RMARS1234567890123456789M",
            LaunchDate = new DateTime(2025, 9, 3),
            Origin = "Earth",
            Destination = "Mars",
            Sender = "Sender",
            Recipient = "Recipient",
            Contents = "Contents"
        };
        Assert.Empty(parcel.History);
        
        // Act
        var timestamp = new DateTime(2025, 9, 3);
        parcel.UpdateState(ParcelState.Created, timestamp);
        
        // Assert
        Assert.Equal(ParcelState.Created, parcel.Status);
        Assert.Single(parcel.History);
        Assert.Equal(ParcelState.Created, parcel.History[0].Status);
        Assert.Equal(timestamp, parcel.History[0].Timestamp);
    }

    [Fact]
    public void UpdateState_OnRocketToMars_BeforeLaunchDate_Throws()
    {
        var parcel = new Parcel
        {
            Barcode = "RMARS1234567890123456789M",
            LaunchDate = new DateTime(2025, 9, 3),
            Origin = "Earth",
            Destination = "Mars",
            Sender = "Sender",
            Recipient = "Recipient",
            Contents = "Contents"
        };
        var timestamp = new DateTime(2025, 9, 1);
        Assert.Throws<ParcelStateException>(() => parcel.UpdateState(ParcelState.OnRocketToMars, timestamp));
    }
}