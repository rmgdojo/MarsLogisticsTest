using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RMParcelTracker.Api.Features.Parcel.Register;

namespace RMParcelTracker.Tests;

public class RegisterParcelApiTests(RmParcelTrackerWebApplicationFactory factory)
    : IClassFixture<RmParcelTrackerWebApplicationFactory>
{
    [Fact]
    public async Task When_Parcel_Registration_Request_Is_Invalid_Return_BadRequest()
    {
        var client = factory.CreateClient();
        var request = new RegisterParcelRequest("barcode", "sender", "receiver", "express", "empty content");
        var content = JsonContent.Create(request);
        var response = await client.PostAsync("parcels", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task When_Parcel_Registration_Request_Is_Valid_Return_Response()
    {
        var client = factory.CreateClient();
        var request = new RegisterParcelRequest("RMARS1234567891234567891B", "sender", "receiver", "express",
            "empty content");
        var content = JsonContent.Create(request);
        var response = await client.PostAsync("parcels", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();

        var parcel = JsonSerializer.Deserialize<RegisterParcelResponse>(
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
        Assert.Equal(90, parcel!.EtaDays);
        Assert.Equal("Created", parcel!.Status);
    }

    [Fact]
    public async Task When_Parcel_Registration_Request_BarCode_Not_Unique_Return_Conflict()
    {
        var client = factory.CreateClient();
        var request = new RegisterParcelRequest("RMARS4234567891234567891B", "sender", "receiver", "express",
            "empty content");
        var content = JsonContent.Create(request);
        var response = await client.PostAsync("parcels", content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        request = new RegisterParcelRequest("RMARS4234567891234567891B", "sender", "receiver", "express",
            "empty content");
        content = JsonContent.Create(request);
        response = await client.PostAsync("parcels", content);
        
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}