namespace RMParcelTracker.Api.Common.Models
{
    public record ParcelAuditTrail(ParcelStatus ParcelStatus, DateOnly StatusChangeDate);
}
