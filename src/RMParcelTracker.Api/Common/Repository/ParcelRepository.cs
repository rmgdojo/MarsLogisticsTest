using RMParcelTracker.Api.Common.Models;

namespace RMParcelTracker.Api.Common.Repository
{
    public class ParcelRepository
    {
        private readonly Dictionary<string, Parcel> _parcels = new();
        public void Add(Parcel parcel)
        {
            if (parcel is null)
            {
                throw new ArgumentNullException(nameof(parcel));
            }

            if (_parcels.TryGetValue(parcel.BarCode, out var _))
            {
                throw new ArgumentException("Parcel already exists");
            }
            
            _parcels.Add(parcel.BarCode, parcel);
        }

        public Parcel? Get(string barcode)
        {
            if (_parcels.TryGetValue(barcode, out var parcel
                ))
            {
                return parcel;
            }

            return null;
        }
    }
}
