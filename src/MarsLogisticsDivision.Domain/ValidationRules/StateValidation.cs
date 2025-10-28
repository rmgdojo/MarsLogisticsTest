using MarsLogisticsDivision.Domain.Enums;

namespace MarsLogisticsDivision.Domain.ValidationRules;

public class StateValidation
{
    public static bool IsValidStateTransition(ParcelState currentState, ParcelState newState)
    {
        return currentState switch
        {
            ParcelState.Created when newState == ParcelState.OnRocketToMars 
                => true,
            ParcelState.OnRocketToMars when newState is ParcelState.LandedOnMars or ParcelState.Lost 
                => true,
            ParcelState.LandedOnMars when newState == ParcelState.OutForMartianDelivery 
                => true,
            ParcelState.OutForMartianDelivery when newState is ParcelState.Delivered or ParcelState.Lost 
                => true,
            _ => false
        };
    }
}