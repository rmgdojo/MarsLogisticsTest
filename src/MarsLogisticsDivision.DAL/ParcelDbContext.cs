using MarsLogisticsDivision.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarsLogisticsDivision.DAL;

public class ParcelDbContext(DbContextOptions<ParcelDbContext> options) : DbContext(options)
{
    public DbSet<Parcel> Parcels => Set<Parcel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Parcel>().HasKey(p => p.Barcode);
        base.OnModelCreating(modelBuilder);
    }
}

