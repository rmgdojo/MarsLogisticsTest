using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RMParcelTracker.Api.Features.Parcel.Register;
using RMParcelTracker.Api.Features.Parcel.Retrieve;

namespace RMParcelTracker.Tests;

public class RetrieveParcelApiTests(RmParcelTrackerWebApplicationFactory factory)
    : IClassFixture<RmParcelTrackerWebApplicationFactory>
{
    [Fact]
    public async Task If_Parcel_Barcode_Not_Valid_Return_Bad_Request()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/parcels/RMARS");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task If_Parcel_Not_Found_Return_Not_Found()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/parcels/RMARS1234567891234565891B");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task If_Parcel_Found_Return_Response()
    {
        var barcode = "RMARS1234567891234567891B";
        var client = factory.CreateClient();
        var request = new RegisterParcelRequest(barcode, "sender", "receiver", "express", "empty content");
        var content = JsonContent.Create(request);
        await client.PostAsync("parcels", content);

        var response = await client.GetAsync($"/parcels/{barcode}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);


        var responseContent = await response.Content.ReadAsStringAsync();

        var parcel = JsonSerializer.Deserialize<RetrieveParcelResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.Equal(request.Barcode, parcel!.Barcode);
        Assert.Equal(request.Sender, parcel!.Sender);
        Assert.Equal(request.Recipient, parcel!.Recipient);
        Assert.Equal(request.Contents, parcel!.Contents);

        Assert.Equal("Starport Thames Estuary", parcel!.Origin);
        Assert.Equal("New London", parcel!.Destination);
        Assert.Equal("2026-02-03", parcel!.EstimatedArrivalDate.ToString("O"));
        Assert.Equal("2025-11-05", parcel!.LaunchDate.ToString("O"));
        Assert.Equal("Created", parcel!.Status);

        var history = parcel.History?.First();
        Assert.Equal("Created", history!.Status);
        Assert.Equal(DateOnly.FromDateTime(new DateTime(2025, 10, 26)), history.TimeStamp);
    }
}