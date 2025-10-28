using RMParcelTracker.Api.Common;

namespace RMParcelTracker.Tests;

public class TestClock(DateTime date) : IClock
{
    private DateTime _currentDateTime = date;

    public DateOnly GetCurrentDate()
    {
        return DateOnly.FromDateTime(_currentDateTime);
    }

    public void Advance(TimeSpan span)
    {
        _currentDateTime = _currentDateTime.Add(span);
    }
}