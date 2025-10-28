using System.Net;
using System.Net.Http.Json;
using RMParcelTracker.Api.Features.Parcel.Register;
using RMParcelTracker.Api.Features.Parcel.Update;

namespace RMParcelTracker.Tests;

public class UpdateParcelApiTests(RmParcelTrackerWebApplicationFactory factory)
    : IClassFixture<RmParcelTrackerWebApplicationFactory>
{
    [Fact]
    public async Task If_Parcel_Barcode_Not_Valid_Return_BadRequest()
    {
        var client = factory.CreateClient();
        var request = new UpdateParcelStatusRequest("OnRocketToMars");
        var content = JsonContent.Create(request);
        var response = await client.PatchAsync("/parcels/RMARS", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task If_Parcel_Not_Found_Return_BadRequest()
    {
        var client = factory.CreateClient();
        var request = new UpdateParcelStatusRequest("OnRocketToMars");
        var content = JsonContent.Create(request);
        var response = await client.PatchAsync("/parcels/RMARS1234567891234567891B", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Parcel_Can_Be_Transitioned_To_Correct_State_On_Express_Service()
    {
        var client = factory.CreateClient();
        var barcode = "RMARS1234567891234567891B";
        var request = new RegisterParcelRequest(barcode, "sender", "receiver", "express", "empty content");
        var content = JsonContent.Create(request);
        await client.PostAsync("parcels", content);

        factory.AdvanceTestClockTime(TimeSpan.FromDays(90));
        var updateRequest = new UpdateParcelStatusRequest("OnRocketToMars");
        content = JsonContent.Create(updateRequest);
        var response = await client.PatchAsync($"/parcels/{barcode}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Parcel_Can_Be_Transitioned_To_Correct_State_On_Standard_Service()
    {
        var client = factory.CreateClient();
        var barcode = "RMARS2234567891234567891B";
        var request = new RegisterParcelRequest(barcode, "sender", "receiver", "standard", "empty content");
        var content = JsonContent.Create(request);
        await client.PostAsync("parcels", content);

        factory.AdvanceTestClockTime(TimeSpan.FromDays(27*30));
        var updateRequest = new UpdateParcelStatusRequest("OnRocketToMars");
        content = JsonContent.Create(updateRequest);
        var response = await client.PatchAsync($"/parcels/{barcode}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}