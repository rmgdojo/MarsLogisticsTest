using MarsLogisticsDivision.Domain.DeliveryModels;
using MarsLogisticsDivision.Domain.Enums;

namespace MarsLogisticsDivision.Domain.Factories;

public static class DeliveryModelFactory
{
    public static IDeliveryModel CreateDeliveryModel(DeliveryService deliveryService)
    {
        return deliveryService switch
        {
            DeliveryService.Standard => new StandardDelivery(),
            DeliveryService.Express => new ExpressDelivery(),
            _ => throw new ArgumentOutOfRangeException(nameof(deliveryService), deliveryService, "Unsupported delivery service type")
        };
    }
}