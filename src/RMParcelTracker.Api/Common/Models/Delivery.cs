namespace RMParcelTracker.Api.Common.Models
{
    public abstract class Delivery
    {
        public abstract int Eta { get; }
        public abstract DeliveryServiceType Type { get; }
        public DateOnly LaunchDate { get; set; }
        public DateOnly EstimatedArrivalDate { get; set; }
    }
}
