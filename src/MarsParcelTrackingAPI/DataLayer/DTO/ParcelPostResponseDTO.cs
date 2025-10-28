using MarsParcelTrackingAPI.DataLayer.Models;
using System.Text.Json.Serialization;

namespace MarsParcelTrackingAPI.DataLayer.DTO;

public class ParcelPostResponseDTO
{
    public string Barcode { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ParcelStatus Status { get; set; }
    
    public string LaunchDate { get; set; }
    
    public int EtaDays { get; set; }
    
    public string EstimatedArrivalDate { get; set; }
    
    public string Origin { get; set; }
    
    public string Destination { get; set; }
    
    public string Sender { get; set; }
    
    public string Recipient { get; set; }
    
    public string Contents { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ParcelPostResponseDTO() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public ParcelPostResponseDTO(Parcel parcel) => (
        Barcode,
        Status,
        LaunchDate,
        EtaDays,
        EstimatedArrivalDate,
        Origin,
        Destination,
        Sender,
        Recipient,
        Contents)
    = (
        parcel.Barcode,
        parcel.Status,
        parcel.LaunchDate.Date.ToString("yyyy-MM-dd"),
        parcel.EtaDays,
        parcel.EstimatedArrivalDate.Date.ToString("yyyy-MM-dd"),
        parcel.Origin,
        parcel.Destination,
        parcel.Sender,
        parcel.Recipient,
        parcel.Contents
    );
}
