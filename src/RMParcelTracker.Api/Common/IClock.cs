namespace RMParcelTracker.Api.Common
{
    public interface IClock
    {
        public DateOnly GetCurrentDate();
    }
}
