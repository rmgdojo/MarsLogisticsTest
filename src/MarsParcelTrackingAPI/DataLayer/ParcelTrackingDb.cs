using MarsParcelTrackingAPI.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace MarsParcelTrackingAPI.DataLayer;

public class ParcelTrackingDb(DbContextOptions<ParcelTrackingDb> options) : DbContext(options)
{
    public DbSet<Parcel> Parcels => Set<Parcel>();

    public DbSet<ParcelHistory> ParcelHistories => Set<ParcelHistory>();
}
