using MarsLogisticsDivision.Domain.Entities;
using MarsLogisticsDivision.Domain.Enums;
using MarsLogisticsDivision.Domain.Utilities;

namespace MarsLogisticsDivision.Domain.DeliveryModels;

public class ExpressDelivery : IDeliveryModel
{
    private const int EtaDays = 90;
    public void InitialiseDelivery(Parcel parcel)
    {
        var launchDate = LaunchDateUtil.GetNextLaunchDateExpress(DateTime.UtcNow);
        
        parcel.LaunchDate = launchDate;
        parcel.EstimatedArrivalDate = launchDate.AddDays(EtaDays);   
        parcel.DeliveryService = DeliveryService.Express;
        parcel.EtaDays = EtaDays;
    }
}