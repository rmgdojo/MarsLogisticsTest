using MarsLogisticsDivision.Domain.Entities;
using MarsLogisticsDivision.Domain.Enums;
using MarsLogisticsDivision.Domain.Utilities;

namespace MarsLogisticsDivision.Domain.DeliveryModels;

public class StandardDelivery : IDeliveryModel
{
    private const int StandardDeliveryDays = 180;
    // In a real implementation, this would probably be fetched from an upstream service or configuration
    private static readonly DateTime NextScehduledLaunchDate = new (2025, 10, 1);

    public void InitialiseDelivery(Parcel parcel)
    {
        var launchDate = LaunchDateUtil.GetNextLaunchDateStandard(DateTime.UtcNow, NextScehduledLaunchDate);
        
        parcel.LaunchDate = launchDate;
        parcel.EstimatedArrivalDate = parcel.LaunchDate.AddDays(StandardDeliveryDays);
        parcel.DeliveryService = DeliveryService.Standard;
        parcel.EtaDays = StandardDeliveryDays;
        
    }
}