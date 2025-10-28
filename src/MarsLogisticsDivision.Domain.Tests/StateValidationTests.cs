using MarsLogisticsDivision.Domain.Enums;
using MarsLogisticsDivision.Domain.ValidationRules;

namespace MarsLogisticsDivision.Domain.Tests;

public class StateValidationTests
{
    [Theory]
    [InlineData(ParcelState.Created, ParcelState.OnRocketToMars, true)]
    [InlineData(ParcelState.OnRocketToMars, ParcelState.LandedOnMars, true)]
    [InlineData(ParcelState.OnRocketToMars, ParcelState.Lost, true)]
    [InlineData(ParcelState.LandedOnMars, ParcelState.OutForMartianDelivery, true)]
    [InlineData(ParcelState.OutForMartianDelivery, ParcelState.Delivered, true)]
    [InlineData(ParcelState.OutForMartianDelivery, ParcelState.Lost, true)]
    [InlineData(ParcelState.Created, ParcelState.Delivered, false)]
    [InlineData(ParcelState.LandedOnMars, ParcelState.Lost, false)]
    public void IsValidStateTransition_ReturnsExpected(ParcelState current, ParcelState next, bool expected)
    {
        Assert.Equal(expected, StateValidation.IsValidStateTransition(current, next));
    }
}