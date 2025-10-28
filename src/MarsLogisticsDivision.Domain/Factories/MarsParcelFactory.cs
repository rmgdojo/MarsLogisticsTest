using MarsLogisticsDivision.Domain.Entities;
using MarsLogisticsDivision.Domain.Enums;

namespace MarsLogisticsDivision.Domain.Factories;

public static class MarsDesignatedParcelFactory
{
    private const string Origin = "Starport Thames Estuary";
    private const string Destination = "New London";
    public static Parcel Build(string barcode, string sender, string recipient, string contents, DateTime dateOfCreation)
    {
        var parcel = new Parcel
        {
            Barcode = barcode,
            Origin = Origin,
            Destination = Destination,
            Sender = sender,
            Recipient = recipient,
            Contents = contents
        };
        parcel.UpdateState(ParcelState.Created, dateOfCreation);
        return parcel;
    }
}