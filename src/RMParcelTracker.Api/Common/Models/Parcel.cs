namespace RMParcelTracker.Api.Common.Models
{
    public class Parcel
    {
        public string BarCode { get; }
        public string Sender { get; }
        public string Recipient { get; }
        public string Contents { get; }
        public string Origin { get; }
        public string Destination { get; }
        public Delivery Delivery { get; }
        
        public IEnumerable<ParcelAuditTrail> History => _auditTrails.AsReadOnly();
        public ParcelStatus Status => _status;
        
        private ParcelStatus _status;
        private static string DefaultOrigin => "Starport Thames Estuary";
        private static string DefaultDestination => "New London";
        private readonly List<ParcelAuditTrail> _auditTrails = new List<ParcelAuditTrail>();
   
        public Parcel(string barcode, string sender, string recipient, string deliveryService, string contents, DateOnly registrationDate)
        {
            ArgumentException.ThrowIfNullOrEmpty(barcode);
            ArgumentException.ThrowIfNullOrEmpty(sender);
            ArgumentException.ThrowIfNullOrEmpty(recipient);
            ArgumentException.ThrowIfNullOrEmpty(deliveryService);
            ArgumentException.ThrowIfNullOrEmpty(contents);

            BarCode = barcode!.Trim();
            Sender = sender!.Trim();
            Recipient = recipient!.Trim();
            Contents = contents!.Trim();

            Origin = DefaultOrigin;
            Destination = DefaultDestination;
            _status = ChangeStatus(nameof(ParcelStatus.Created), registrationDate);
            Delivery = SetDeliveryDetails(deliveryService, registrationDate);
        }

        private Delivery SetDeliveryDetails(string deliveryService, DateOnly registrationDate)
        {
            return deliveryService.ToLower() switch
            {
                "standard" => new StandardDelivery(registrationDate),
                "express" => new ExpressDelivery(registrationDate),
                _ => throw new ArgumentException("Invalid delivery service type")
            };
        }

        public ParcelStatus ChangeStatus(string nextStatus, DateOnly statusChangeDate)
        {
           var nextParcelStatus = ParcelStatusConverter.GetParcelStatus(nextStatus);
           if (nextParcelStatus is null)
           {
               throw new ArgumentException("Invalid parcel status");
           }
           
           if(nextParcelStatus == ParcelStatus.Created && !History.Any())
           {
               _auditTrails.Add(new ParcelAuditTrail(nextParcelStatus.Value, statusChangeDate));
               return nextParcelStatus.Value;
           }

           var statusTransitionResult =
               ParcelStateTransition.CanTransitionToState(Status, nextParcelStatus.Value,Delivery.LaunchDate, statusChangeDate);

           if (statusTransitionResult.IsFailed)
           {
               throw new ArgumentException(string.Join(",",statusTransitionResult.Errors.Select(s=>s.Message)));
           }
           
           _auditTrails.Add(new ParcelAuditTrail(nextParcelStatus.Value, statusChangeDate));

           _status = nextParcelStatus.Value;
           
            return nextParcelStatus.Value;
        }
    }
}
