namespace MarsParcelTrackingAPI;

public enum ParcelStatus
{
    NotSet = 0,
    Created = 1,
    OnRocketToMars = 2,
    LandedOnMars = 3,
    OutForMartianDelivery = 4,
    Delivered = 5,
    Lost = 6
}

public enum DeliveryService
{
    NotSet = 0,
    Standard = 1,
    Express = 2
}
