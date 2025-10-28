using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MarsParcelTrackingAPI.DataLayer.Models;

public class Parcel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public required string Barcode { get; set; }

    public required string Sender { get; set; }

    public required string Recipient { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required DeliveryService DeliveryService { get; set; } = DeliveryService.NotSet;
    
    public required string Contents { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ParcelStatus Status { get; set; }

    public required DateTime LaunchDate { get; set; }

    public required DateTime EstimatedArrivalDate { get; set; }

    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public required int EtaDays { get; set; }

    public ICollection<ParcelHistory>? ParcelHistories { get; } = [];
}
