namespace RMParcelTracker.Api.Common.Models;

public class StandardDelivery : Delivery
{
    public sealed override int Eta => 180;
    public override DeliveryServiceType Type => DeliveryServiceType.Standard;
    private static readonly DateOnly FirstLaunchDate = new DateOnly(2025, 10, 1);
    private static int IntervalMonths => 26;

    public StandardDelivery(DateOnly parcelRegistrationDate)
    {
        if (parcelRegistrationDate <= FirstLaunchDate)
        {
            LaunchDate = FirstLaunchDate;
        }
        else
        {
            var nextLaunch = FirstLaunchDate;
            while (nextLaunch < parcelRegistrationDate)
            {
                nextLaunch = nextLaunch.AddMonths(IntervalMonths);
            }
            
            LaunchDate = nextLaunch;
        }

        EstimatedArrivalDate = LaunchDate.AddDays(Eta);
    }
}