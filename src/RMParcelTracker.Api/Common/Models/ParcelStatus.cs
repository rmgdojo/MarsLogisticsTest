namespace RMParcelTracker.Api.Common.Models
{
    public enum ParcelStatus
    {
        Created = 1,
        OnRocketToMars = 2,
        LandedOnMars = 3,
        OutForMartianDelivery = 4,
        Delivered = 5,
        Lost = 6
    };

    public static class ParcelStatusConverter
    {
        public static ParcelStatus? GetParcelStatus(string parcelStatus)
        {
            var parseResult= Enum.TryParse<ParcelStatus>(parcelStatus, ignoreCase: true, out var enumParcelStatus);
            if (!parseResult)
            {
                return null;
            }

            return enumParcelStatus;
        }
    }
}
