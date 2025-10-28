using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MarsParcelTrackingAPI.DataLayer.Models;

public class ParcelHistory
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    // NOTE: don't include a direct link to the Parcel class, or it creates circular references
    public long ParcelID { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ParcelStatus Status { get; set; }

    public required DateTime Timestamp { get; set; }
}
