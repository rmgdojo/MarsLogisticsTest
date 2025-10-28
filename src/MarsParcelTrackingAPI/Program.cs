using MarsParcelTrackingAPI;
using MarsParcelTrackingAPI.DataLayer;
using MarsParcelTrackingAPI.DataLayer.DTO;
using MarsParcelTrackingAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Parcel Tracking DB Entity Framework context
builder.Services.AddDbContextFactory<ParcelTrackingDb>(options => options.UseInMemoryDatabase("MarsParcelTrackingDB"));

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClock, EarthClock>();

// Convert enums to string
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddTransient<IParcelTrackingService, ParcelTrackingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Use Swagger only in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Create seed data as required - THIS HAS DELIBERATELY LEFT HERE!
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();

//    var context = scope.ServiceProvider.GetRequiredService<ParcelTrackingDb>();

//    var seeder = new DataSeed(context);

//    await seeder.SeedData();
//}

if (app.Environment.IsDevelopment())
{
    // These methods are only provided to assist with testing via Swagger
    app.MapGet("/getcurrentearthtime", (IClock clock) =>
    {
        return Results.Ok(clock.UtcNow);
    });

    app.MapPost("/setcurrentearthtime", (SetCurrentEarthTimePostRequestDTO requestDTO, IClock clock) =>
    {
        if (string.IsNullOrWhiteSpace(requestDTO.UpdatedEarthTime))
        {
            return Results.BadRequest("Empty parameter");
        }

        clock.SetCurrentEarthTime(DateTime.Parse(requestDTO.UpdatedEarthTime));

        return Results.Ok();
    });
}

app.MapPost("/parcels", async (ParcelPostRequestDTO parcelRequest, IParcelTrackingService parcelTrackingService) =>
    {
        var (responseDTO, errorMessage) = await parcelTrackingService.CreateNewParcelAsync(parcelRequest);

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            return Results.BadRequest(errorMessage);
        }

        if (responseDTO is null)
        {
            return Results.BadRequest("Null response");
        }

        return TypedResults.Created($"/parcels/{responseDTO.Barcode}", responseDTO);
    });

app.MapMethods(
    "/parcels/{barcode}",
    ["PATCH"],
    async ([FromRoute] string barcode, [FromBody] ParcelPatchRequestDTO patchRequestDTO, IParcelTrackingService parcelTrackingService, IClock clock) =>
    {
        var errorMessage = await parcelTrackingService.UpdateParcelStatusAsync(barcode, patchRequestDTO);

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            return Results.BadRequest(errorMessage);
        }

        return Results.Ok();
    });

app.MapGet("/parcels/{barcode}", async (string barcode, IParcelTrackingService parcelTrackingService) =>
    {
        var (responseDTO, errorMessage) = await parcelTrackingService.GetParcelDetailsAsync(barcode);

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            return Results.BadRequest(errorMessage);
        }

        return Results.Ok(responseDTO);
    });

// Start the app
app.Run();
