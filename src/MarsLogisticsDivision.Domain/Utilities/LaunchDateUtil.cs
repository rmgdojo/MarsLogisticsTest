namespace MarsLogisticsDivision.Domain.Utilities;

public static class LaunchDateUtil
{
    private const int StandardLaunchCycleMonths = 26;
    
    /// <summary>
    /// Returns next First (selected day) of the Month
    /// </summary>
    /// <param name="today"></param>
    /// <param name="launchDay"><param>
    /// <returns></returns>
    public static DateTime GetNextLaunchDateExpress(DateTime today, DayOfWeek launchDay = DayOfWeek.Wednesday)
    {
        var firstThisMonth = FirstLaunchDayOfMonth(today.Year, today.Month, launchDay);
        return firstThisMonth >= today.Date
            ? firstThisMonth
            : FirstLaunchDayOfMonth(today.Year, today.Month + 1, launchDay);
    }

    /// <summary>
    /// Returns next scheduled launch date or if date passed, the next cycle estimate
    /// </summary>
    /// <param name="currentDate"></param>
    /// <param name="nextLaunchDate"></param>
    /// <returns></returns>
    public static DateTime GetNextLaunchDateStandard(DateTime currentDate, DateTime nextLaunchDate)
    {
        return currentDate < nextLaunchDate ? nextLaunchDate : nextLaunchDate.AddMonths(StandardLaunchCycleMonths);
    }
    
    private static DateTime FirstLaunchDayOfMonth(int year, int month, DayOfWeek launchDayOfWeek)
    {
        var first = new DateTime(year, month, 1);
        while (first.DayOfWeek != launchDayOfWeek)
        {
            first = first.AddDays(1);
        }
        return first;
    }
}