using MarsParcelTrackingAPI.DataLayer;
using MarsParcelTrackingAPI.DataLayer.DTO;
using MarsParcelTrackingAPI.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace MarsParcelTrackingAPI.Services;

public interface IParcelTrackingService
{
    Task<(ParcelPostResponseDTO? responseDTO, string errorMessage)> CreateNewParcelAsync(ParcelPostRequestDTO parcelRequest);

    Task<(ParcelGetResponseDTO? responseDTO, string errorMessage)> GetParcelDetailsAsync(string barcode);
    
    Task<string> UpdateParcelStatusAsync(string barcode, ParcelPatchRequestDTO patchRequestDTO);
}

public class ParcelTrackingService(
    IClock clock,
    IDbContextFactory<ParcelTrackingDb> contextFactory) : IParcelTrackingService
{
    public async Task<(ParcelPostResponseDTO? responseDTO, string errorMessage)> CreateNewParcelAsync(ParcelPostRequestDTO parcelRequest)
    {
        if (!parcelRequest.Barcode.IsValidBarcode())
        {
            return (null, Constants.ERROR_INVALID_BARCODE);
        }

        var (launchDate, etaDays) = parcelRequest.DeliveryService.ProcessSelectedDeliveryService(clock.UtcNow);
        
        if (launchDate is null || etaDays is null)
        {
            return (null, Constants.ERROR_INVALID_DELIVERY_SERVICE);
        }

        // Calculate the Estimated Arrival Date
        var arrivalDate = launchDate.Value.AddDays(etaDays.Value);

        var parcelHistory = new ParcelHistory
        {
            Status = ParcelStatus.Created,
            Timestamp = clock.UtcNow
        };

        var parcel = new Parcel
        {
            Barcode = parcelRequest.Barcode,
            Contents = parcelRequest.Contents,
            DeliveryService = parcelRequest.DeliveryService,
            Destination = Constants.PARCEL_DESTINATION,
            EtaDays = etaDays.Value,
            EstimatedArrivalDate = arrivalDate,
            LaunchDate = launchDate.Value.ToUniversalTime(),
            Origin = Constants.PARCEL_ORIGIN,
            Recipient = parcelRequest.Recipient,
            Sender = parcelRequest.Sender,
            Status = ParcelStatus.Created
        };

        using var db = await contextFactory.CreateDbContextAsync();

        await db.Parcels.AddAsync(parcel);

        await db.SaveChangesAsync();

        // Update history records with the current Parcel ID.
        parcelHistory.ParcelID = parcel.Id;

        await db.ParcelHistories.AddAsync(parcelHistory);

        await db.SaveChangesAsync();

        var parcelResponseDTO = new ParcelPostResponseDTO(parcel);

        return (parcelResponseDTO, string.Empty);
    }

    public async Task<(ParcelGetResponseDTO? responseDTO, string errorMessage)> GetParcelDetailsAsync(string barcode)
    {
        if (!barcode.IsValidBarcode())
        {
            return (null, Constants.ERROR_INVALID_BARCODE);
        }

        using var db = await contextFactory.CreateDbContextAsync();

        var parcel = await db.Parcels
            .Include(i => i.ParcelHistories)
            .Where(w => w.Barcode == barcode)
            .FirstOrDefaultAsync();

        if (parcel is null)
        {
            return (null, Constants.ERROR_PARCEL_NOT_FOUND);
        }

        return (new ParcelGetResponseDTO(parcel), string.Empty);
    }

    public async Task<string> UpdateParcelStatusAsync(string barcode, ParcelPatchRequestDTO patchRequestDTO)
    {
        if (!Enum.TryParse(typeof(ParcelStatus), patchRequestDTO.NewStatus, out var parsedStatus))
        {
            return Constants.ERROR_INVALID_PARCEL_STATUS;
        }

        if (!barcode.IsValidBarcode())
        {
            return Constants.ERROR_INVALID_BARCODE;
        }

        using var db = await contextFactory.CreateDbContextAsync();

        var parcel = await db.Parcels
            .Where(w => w.Barcode == barcode)
            .FirstOrDefaultAsync();

        if (parcel is null)
        {
            return Constants.ERROR_PARCEL_NOT_FOUND;
        }

        var currentStatus = parcel.Status;
        var newStatus = (ParcelStatus)parsedStatus;
        var invalidStatusMessage = $"Invalid status value, current status is {currentStatus}";

        var errorMessage = string.Empty;

        switch (currentStatus)
        {
            case ParcelStatus.Created:
                if (newStatus == ParcelStatus.OnRocketToMars)
                {
                    // check that launch has taken place
                    if (clock.UtcNow < parcel.LaunchDate)
                    {
                        errorMessage = $"Launch date of {parcel.LaunchDate.Date:yyyy-MM-dd} has not occurred";
                    }
                }
                else
                {
                    errorMessage = invalidStatusMessage;
                }
                break;
            case ParcelStatus.OnRocketToMars:
                if (newStatus != ParcelStatus.LandedOnMars & newStatus != ParcelStatus.Lost)
                {
                    errorMessage = invalidStatusMessage;
                }
                if (newStatus == ParcelStatus.LandedOnMars)
                {
                    // Check the dates to ensure that the rocket has potentially reached Mars.
                    if (clock.UtcNow < parcel.EstimatedArrivalDate)
                    {
                        errorMessage = $"Estimated Arrival Date of {parcel.EstimatedArrivalDate.Date:yyyy-MM-dd} is in the future";
                    }
                }
                break;
            case ParcelStatus.LandedOnMars:
                if (newStatus != ParcelStatus.OutForMartianDelivery)
                {
                    errorMessage = invalidStatusMessage;
                }
                break;
            case ParcelStatus.OutForMartianDelivery:
                if (newStatus != ParcelStatus.Delivered & newStatus != ParcelStatus.Lost)
                {
                    errorMessage = invalidStatusMessage;
                }
                break;
            case ParcelStatus.Delivered:
                errorMessage = "Parcel has already been delivered";
                break;
            default:
                errorMessage = invalidStatusMessage;
                break;
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            var parcelHistory = new ParcelHistory
            {
                Status = newStatus,
                Timestamp = clock.UtcNow,
                ParcelID = parcel.Id
            };

            await db.ParcelHistories.AddAsync(parcelHistory);

            parcel.Status = newStatus;

            db.Parcels.Update(parcel);

            await db.SaveChangesAsync();
        }

        return errorMessage;
    }
}
