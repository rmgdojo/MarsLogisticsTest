using MarsLogisticsDivision.Domain.Entities;

namespace MarsLogisticsDivision.Domain.DeliveryModels;

public interface IDeliveryModel
{
    public void InitialiseDelivery(Parcel parcel);
}