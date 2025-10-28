using MarsLogisticsDivision.Application.DTO;
using MarsLogisticsDivision.Domain.Factories;
using MarsLogisticsDivision.DAL;
using MarsLogisticsDivision.Domain.Entities;
using MarsLogisticsDivision.Domain.Enums;
using MarsLogisticsDivision.Domain.Exceptions;
using MarsLogisticsDivision.Domain.Providers;
using MarsLogisticsDivision.Domain.ValidationRules;
using Microsoft.EntityFrameworkCore;

namespace MarsLogisticsDivision.Application.Services;

public interface IParcelService
{
    public Task<Parcel> RecordDelivery(ParcelCreationData parcel);
    public Task UpdateParcelStatus(string barcode, ParcelState newStatus);
    public Task<Parcel> GetParcelByBarcode(string barcode)
    {
        throw new NotImplementedException();
    }
}

public class ParcelService(ParcelDbContext dbContext, IDateTimeProvider timeProvider) : IParcelService
{
    public async Task<Parcel> RecordDelivery(ParcelCreationData data)
    {
        var existingParcel = await dbContext.Parcels.FindAsync(data.Barcode);
        if (existingParcel != null)
            throw new BarcodeException($"Parcel with barcode {data.Barcode} already exists.");
        var parcel = MarsDesignatedParcelFactory.Build(data.Barcode, data.Sender, data.Contents, data.Recipient, timeProvider.UtcNow);
        
        var deliveryModel = DeliveryModelFactory.CreateDeliveryModel(data.DeliveryService);
        deliveryModel.InitialiseDelivery(parcel);
        
        await dbContext.Parcels.AddAsync(parcel);
        await dbContext.SaveChangesAsync();
        
        return parcel;
    }
    
    public async Task UpdateParcelStatus(string barcode, ParcelState newStatus)
    {
        var parcel = await dbContext.Parcels.FindAsync(barcode);
        if (parcel == null)
            throw new KeyNotFoundException($"Parcel with barcode {barcode} not found.");

        if (!StateValidation.IsValidStateTransition(parcel.Status, newStatus))
            throw new ParcelStateException($"Invalid state transition from {parcel.Status} to {newStatus}.");
        
        parcel.UpdateState(newStatus, timeProvider.UtcNow); 
        
        dbContext.Parcels.Update(parcel);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task<Parcel> GetParcelByBarcode(string barcode)
    {
        var parcel = await dbContext.Parcels
            .Include(p => p.History)
            .FirstOrDefaultAsync(p => p.Barcode == barcode);

        if (parcel == null)
            throw new KeyNotFoundException($"Parcel with barcode {barcode} not found.");
        return parcel;
    }
}