using MarsLogisticsDivision.Domain.Utilities;

namespace MarsLogisticsDivision.Domain.Tests;

public class LaunchDateUtilTests
{
    [Fact]
    public void GetNextLaunchDateExpress_ReturnsCorrectDay()
    {
        var today = new DateTime(2025, 10, 25); // Saturday
        var nextLaunch = LaunchDateUtil.GetNextLaunchDateExpress(today, DayOfWeek.Wednesday);
        Assert.Equal(DayOfWeek.Wednesday, nextLaunch.DayOfWeek);
        Assert.True(nextLaunch >= today);
    }

    [Fact]
    public void GetNextLaunchDateStandard_ReturnsNextCycleIfPast()
    {
        var currentDate = new DateTime(2025, 10, 25);
        var nextLaunchDate = new DateTime(2025, 9, 3);
        var result = LaunchDateUtil.GetNextLaunchDateStandard(currentDate, nextLaunchDate);
        Assert.Equal(nextLaunchDate.AddMonths(26), result);
    }

    [Fact]
    public void GetNextLaunchDateStandard_ReturnsNextLaunchIfFuture()
    {
        var currentDate = new DateTime(2025, 8, 1);
        var nextLaunchDate = new DateTime(2025, 9, 3);
        var result = LaunchDateUtil.GetNextLaunchDateStandard(currentDate, nextLaunchDate);
        Assert.Equal(nextLaunchDate, result);
    }
}