namespace MarsParcelTrackingAPI;

public interface IClock
{
    DateTime UtcNow { get; }

    void SetCurrentEarthTime(DateTime updatedDateTime);
}

public class EarthClock : IClock
{
    private DateTime _utcNow;

    public EarthClock()
    {
        _utcNow = DateTime.UtcNow;
    }

    public DateTime UtcNow => _utcNow;

    public void SetCurrentEarthTime(DateTime updatedDateTime)
    {
        _utcNow = updatedDateTime;
    }
}