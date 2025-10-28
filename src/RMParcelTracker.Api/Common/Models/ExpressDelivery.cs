namespace RMParcelTracker.Api.Common.Models;

public class ExpressDelivery : Delivery
{
    public sealed override int Eta => 90;
    public override DeliveryServiceType Type => DeliveryServiceType.Express;
    
    public ExpressDelivery(DateOnly parcelRegistrationDate)
    {
        var firstWednesdayOfGivenMonth = FirstWednesdayOfGivenDate(parcelRegistrationDate);
        if (parcelRegistrationDate <= firstWednesdayOfGivenMonth)
        {
            LaunchDate = firstWednesdayOfGivenMonth;
        }
        else
        {
            var firstWednesdayOfNextMonth = FirstWednesdayOfGivenDate(parcelRegistrationDate.AddMonths(1));
            LaunchDate = firstWednesdayOfNextMonth;
        }

        EstimatedArrivalDate = LaunchDate.AddDays(Eta);
    }

    private DateOnly FirstWednesdayOfGivenDate(DateOnly date)
    {
        var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        int daysToAdd = ((int)DayOfWeek.Wednesday - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
        return DateOnly.FromDateTime(firstDayOfMonth.AddDays(daysToAdd));
    }
}