using RMParcelTracker.Api.Common.Models;
namespace RMParcelTracker.Tests
{
    public class ExpressDeliveryTests
    {
        [Theory]
        [InlineData("2025-11-01", "2025-11-05")] // registration before first Wed of the month returns same month's first Wed
        [InlineData("2025-11-05", "2025-11-05")] // registration on first Wed of the month returns same month's first Wed
        [InlineData("2025-10-26", "2025-11-05")] // registration after first Wed of the month returns next month's first Wed
        public void LaunchDate_Is_Calculated_Correctly_For_Express(string registrationDate, string expectedLaunchDate)
        {
            var registration = DateOnly.Parse(registrationDate);
            var expected = DateOnly.Parse(expectedLaunchDate);

            var delivery = new ExpressDelivery(registration);

            Assert.Equal(expected, delivery.LaunchDate);
        }
    }
}
