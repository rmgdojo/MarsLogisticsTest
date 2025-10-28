namespace RMParcelTracker.Api.Common
{
    public class SystemClock : IClock
    {
        public DateOnly GetCurrentDate()
        {
            return DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }
}
