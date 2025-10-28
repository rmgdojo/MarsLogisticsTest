using RMParcelTracker.Api.Common.Models;

namespace RMParcelTracker.Tests
{
    public class StandardDeliveryTests
    {
        [Theory]
        [InlineData("2025-09-30", "2025-10-01")] // before first launch return first launch
        [InlineData("2025-10-01", "2025-10-01")] // on same date of first launch , return same
        [InlineData("2025-10-02", "2027-12-01")] // after the launch date -> next scheduled (26 months) launch date
        public void LaunchDate_Is_Calculated_Correctly_For_Standard(string registrationDate, string expectedLaunchDate)
        {
            var registration = DateOnly.Parse(registrationDate);
            var expected = DateOnly.Parse(expectedLaunchDate);

            var delivery = new StandardDelivery(registration);
            
            Assert.Equal(expected, delivery.LaunchDate);
        }
    }
}
