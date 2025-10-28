namespace RMParcelTracker.Api.Features.Parcel.Register;

public record RegisterParcelResponse(
    string Barcode,
    string Status,
    DateOnly LaunchDate,
    int EtaDays,
    DateOnly EstimatedArrivalDate,
    string Origin,
    string Destination,
    string Sender,
    string Recipient,
    string Contents);

public class RegisterParcelResponseMapper
{
    public static RegisterParcelResponse MapFrom(Common.Models.Parcel parcel)
    {
        return new RegisterParcelResponse(
            parcel.BarCode,
            parcel.Status.ToString(),
            parcel.Delivery.LaunchDate,
            parcel.Delivery.Eta,
            parcel.Delivery.EstimatedArrivalDate,
            parcel.Origin,
            parcel.Destination,
            parcel.Sender,
            parcel.Recipient,
            parcel.Contents
        );
    }
}