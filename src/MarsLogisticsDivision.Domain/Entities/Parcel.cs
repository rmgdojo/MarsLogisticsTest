using System.ComponentModel.DataAnnotations;
using MarsLogisticsDivision.Domain.Enums;
using MarsLogisticsDivision.Domain.Exceptions;

namespace MarsLogisticsDivision.Domain.Entities;

public class Parcel
{
    [Key]
    public string Barcode { get; init; }
    public ParcelState Status { get; private set; }
    public DateTime LaunchDate { set; get; }
    public DateTime EstimatedArrivalDate { get; set; }
    public int EtaDays { get; set; }
    public DeliveryService DeliveryService { get; set; }
    public string Origin { get; init; }
    public string Destination { get; init; }
    public string Sender { get; init; }
    public string Recipient { get; init; }
    public string Contents { get; init; }
    public List<ParcelHistoryEntry> History { get; init; } = [];
    
    public void UpdateState(ParcelState newState, DateTime timestamp)
    {
        if (newState == ParcelState.OnRocketToMars && timestamp < LaunchDate)
        {
            throw new ParcelStateException($"Cannot set status to OnRocketToMars before the launch date ({LaunchDate:yyyy-MM-dd}).");
        }
        Status = newState;
        History.Add(new ParcelHistoryEntry
        {
            Status = newState,
            Timestamp = timestamp
        });
    }
}

public class ParcelHistoryEntry
{
    public int Id { get; set; }
    public ParcelState Status { get; init; }
    public DateTime Timestamp { get; init; }
}
