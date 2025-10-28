using FluentResults;
using RMParcelTracker.Api.Common;

namespace RMParcelTracker.Api.Features.Parcel.Register;

public class RegisterParcel(IClock clock)
{
    public Result<Common.Models.Parcel> Handle(RegisterParcelRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var registerParcelResult = Result.Try(() => new Common.Models.Parcel(request.Barcode, request.Sender,
            request.Recipient,
            request.DeliveryService, request.Contents, clock.GetCurrentDate()));

        if (registerParcelResult.IsFailed) return Result.Fail<Common.Models.Parcel>(registerParcelResult.Errors);

        return Result.Ok(registerParcelResult.Value);
    }
}