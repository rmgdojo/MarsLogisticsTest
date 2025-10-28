using System.Text.Json.Serialization;

namespace MarsParcelTrackingAPI.DataLayer.DTO;

public class ParcelPostRequestDTO
{
    public required string Barcode { get; set; }

    public required string Sender { get; set; }

    public required string Recipient { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required DeliveryService DeliveryService { get; set; }

    public required string Contents { get; set; }
}
