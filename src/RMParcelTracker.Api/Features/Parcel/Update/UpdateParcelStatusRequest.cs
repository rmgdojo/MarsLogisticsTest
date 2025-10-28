namespace RMParcelTracker.Api.Features.Parcel.Update;

public record UpdateParcelStatusRequest(string NewStatus);

public record UpdateParcelStatusRequestWithBarCode(string BarCode, string StatusUpdate);