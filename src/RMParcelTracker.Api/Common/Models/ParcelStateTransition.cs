using FluentResults;

namespace RMParcelTracker.Api.Common.Models;

public static class ParcelStateTransition
{
    private static readonly Dictionary<ParcelStatus, ParcelStatus[]> ValidTransitions =
        new()
        {
            { ParcelStatus.Created, [ParcelStatus.OnRocketToMars] },
            { ParcelStatus.OnRocketToMars, [ParcelStatus.LandedOnMars, ParcelStatus.Lost] },
            { ParcelStatus.LandedOnMars, [ParcelStatus.OutForMartianDelivery] },
            { ParcelStatus.OutForMartianDelivery, [ParcelStatus.Delivered, ParcelStatus.Lost] },
            { ParcelStatus.Delivered, [] },
            { ParcelStatus.Lost, [] }
        };

    public static Result CanTransitionToState(ParcelStatus from, ParcelStatus to, DateOnly launchDate,
        DateOnly currentDate)
    {
        if (!ValidTransitions.TryGetValue(from, out var allowedTransition))
            return Result.Fail("Not a valid transition");

        var transitionAllowed = allowedTransition.Contains(to);
        if (!transitionAllowed) return Result.Fail($" Transition from {from} state to {to} not allowed");

        if (from == ParcelStatus.Created && to == ParcelStatus.OnRocketToMars)
            if (launchDate > currentDate)
                return Result.Fail(
                    $" Transition from {from} state to {to} not allowed as launch has not occured. Launch is scheduled for {launchDate.ToString("O")}");

        return Result.Ok();
    }
}