namespace RMParcelTracker.Api.Features.Parcel.Retrieve;

public record RetrieveParcelResponse(
    string Barcode,
    string Status,
    DateOnly LaunchDate,
    DateOnly EstimatedArrivalDate,
    string Origin,
    string Destination,
    string Sender,
    string Recipient,
    string Contents,
    IEnumerable<ParcelAuditTrailData>? History);

public record ParcelAuditTrailData(string Status, DateOnly TimeStamp);

public class RetrieveParcelResponseMapper
{
    public static RetrieveParcelResponse? MapFrom(Common.Models.Parcel? parcel)
    {
        if (parcel is null) return null;

        return new RetrieveParcelResponse(
            parcel.BarCode,
            parcel.Status.ToString(),
            parcel.Delivery.LaunchDate,
            parcel.Delivery.EstimatedArrivalDate,
            parcel.Origin,
            parcel.Destination,
            parcel.Sender,
            parcel.Recipient,
            parcel.Contents,
            parcel.History?.Select(s => new ParcelAuditTrailData(s.ParcelStatus.ToString(), s.StatusChangeDate))
        );
    }
}