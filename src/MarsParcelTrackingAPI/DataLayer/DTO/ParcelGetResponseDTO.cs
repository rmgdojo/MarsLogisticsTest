using MarsParcelTrackingAPI.DataLayer.Models;

namespace MarsParcelTrackingAPI.DataLayer.DTO;

public class ParcelGetResponseDTO
{
    public string? Barcode { get; set; }

    public string? Status {  get; set; }
    
    public string? LaunchDate { get; set; }

    public string? EstimatedArrivalDate { get; set; }

    public string? Origin { get; set; }

    public string? Destination { get; set; }

    public string? Sender { get; set; }

    public string? Recipient { get; set; }

    public string? Contents { get; set; }

    public List<ParcelHistoryResponseDTO>? History { get; set; }

    public ParcelGetResponseDTO() { }

    public ParcelGetResponseDTO(Parcel parcel)
    {
        this.Barcode = parcel.Barcode;
        this.Contents = parcel.Contents;
        this.Destination = parcel.Destination;
        this.EstimatedArrivalDate = parcel.EstimatedArrivalDate.Date.ToString("yyyy-MM-dd");
        this.LaunchDate = parcel.LaunchDate.Date.ToString("yyyy-MM-dd");
        this.Origin = parcel.Origin;
        this.Recipient = parcel.Recipient;
        this.Sender = parcel.Sender;
        this.Status = Enum.GetName(parcel.Status);

        if (parcel.ParcelHistories is null)
        {
            this.History = [];
        }
        else
        {
            this.History = [.. parcel.ParcelHistories
                .Select(s => new ParcelHistoryResponseDTO
                {
                    Status = Enum.GetName(s.Status),
                    Timestamp = s.Timestamp.Date.ToString("yyyy-MM-dd")
                })];
        }
    }
}
