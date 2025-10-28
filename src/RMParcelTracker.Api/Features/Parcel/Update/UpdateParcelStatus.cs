using FluentResults;
using RMParcelTracker.Api.Common;
using RMParcelTracker.Api.Common.Repository;

namespace RMParcelTracker.Api.Features.Parcel.Update;

public class UpdateParcelStatus(ParcelRepository parcelRepository, IClock clock)
{
    public Result Handle(UpdateParcelStatusRequestWithBarCode request)
    {
        var parcel = parcelRepository.Get(request.BarCode);

        if (parcel is null) return Result.Fail("Parcel not found");

        var changeStatusResult = Result.Try(() => parcel.ChangeStatus(request.StatusUpdate, clock.GetCurrentDate()));

        if (changeStatusResult.IsFailed) return changeStatusResult.ToResult();

        return Result.Ok();
    }
}