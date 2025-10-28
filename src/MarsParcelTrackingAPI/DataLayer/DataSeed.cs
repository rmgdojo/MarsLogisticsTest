using MarsParcelTrackingAPI.DataLayer.Models;

namespace MarsParcelTrackingAPI.DataLayer;

public class DataSeed(ParcelTrackingDb parcelTrackingDb, IClock clock)
{
    public async Task SeedData()
    {
        var parcelTestData = GetParcelTestData();

        await parcelTrackingDb.Parcels.AddRangeAsync(parcelTestData);

        await parcelTrackingDb.SaveChangesAsync();

        foreach (var parcel in parcelTestData)
        {
            await parcelTrackingDb.ParcelHistories.AddAsync(GetParcelHistoryTestData(parcel));
        }

        await parcelTrackingDb.SaveChangesAsync();
    }

    private ParcelHistory GetParcelHistoryTestData(Parcel parcel)
    {
        return new ParcelHistory
        {
            ParcelID = parcel.Id,
            Status = parcel.Status,
            Timestamp = clock.UtcNow
        };
    }

    private static List<Parcel> GetParcelTestData()
    {
        return
        [
            new() {
                Barcode = "RMARS1234567890123456789M",
                Contents = "Signed C# language specification and a Christmas card",
                DeliveryService = DeliveryService.Standard,
                Destination = "New London",
                EstimatedArrivalDate = DateTime.Parse("2025-09-03").AddDays(90),
                EtaDays = 90,
                LaunchDate = DateTime.Parse("2025-09-03"),
                Origin = "Starport Thames Estuary",
                Recipient = "Elon Musk",
                Sender = "Anders Hejlsberg",
                Status = ParcelStatus.Created
            }
        ];
    }
}
