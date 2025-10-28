using System.Text.RegularExpressions;

namespace MarsParcelTrackingAPI;

public static partial class Extensions
{
    [GeneratedRegex("RMARS[0-9]{19}[A-Z]")]
    private static partial Regex BarcodeRegex();

    public static bool IsValidBarcode(this string barcodeToValidate)
    {
        var regEx = BarcodeRegex();

        var match = regEx.Match(barcodeToValidate);

        return match.Success;
    }

    public static DateTime GetNextExpressDeliveryLaunchDate(this DateTime today)
    {
        var firstWed = GetFirstWednesday(today.Year, today.Month);

        while (firstWed < today)
        {
            var year = today.Year;
            var month = today.Month + 1;

            if (month > 12)
            {
                // handle dates that might roll over into next year
                year++;
                month = 1;
            }

            var nextMonth = new DateTime(year, month, 1);

            firstWed = GetFirstWednesday(nextMonth.Year, nextMonth.Month);
        }

        return firstWed;
    }

    private static DateTime GetFirstWednesday(int year, int month)
    {
        // Start at the first day of the given month
        var firstDayOfMonth = new DateTime(year, month, 1);

        // Calculate the offset to the first Wednesday
        var daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)firstDayOfMonth.DayOfWeek + 7) % 7;

        // Return the date of the first Wednesday
        return firstDayOfMonth.AddDays(daysUntilWednesday);
    }

    public static (DateTime? launchDate, int? etaDays) ProcessSelectedDeliveryService(this DeliveryService deliveryService, DateTime utcNow)
    {
        switch (deliveryService)
        {
            case DeliveryService.Standard:
                var standardLaunchDate = new DateTime(2025, 10, 1);
                if (utcNow < standardLaunchDate)
                {
                    // Go back in time to find the relevant launch date.
                    var calculatedLaunchDate = standardLaunchDate.AddMonths(-26);

                    while (utcNow < calculatedLaunchDate)
                    {
                        calculatedLaunchDate = standardLaunchDate.AddMonths(-26);
                    }

                    standardLaunchDate = calculatedLaunchDate;
                }

                var launchDate = standardLaunchDate;

                while (launchDate < utcNow)
                {
                    launchDate = launchDate.AddMonths(26);
                }

                return (launchDate, 180);
            case DeliveryService.Express:
                return (utcNow.GetNextExpressDeliveryLaunchDate(), 90);
            default:
                return (null, null);
        }
    }
}
