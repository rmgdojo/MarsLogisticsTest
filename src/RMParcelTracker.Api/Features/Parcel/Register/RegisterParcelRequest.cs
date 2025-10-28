namespace RMParcelTracker.Api.Features.Parcel.Register;

public record RegisterParcelRequest(
    string Barcode,
    string Sender,
    string Recipient,
    string DeliveryService,
    string Contents);