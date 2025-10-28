using MarsParcelTrackingAPI;
using MarsParcelTrackingAPI.DataLayer;
using MarsParcelTrackingAPI.DataLayer.DTO;
using MarsParcelTrackingAPI.DataLayer.Models;
using MarsParcelTrackingAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MarsParcelTrackerTests
{
    public class ParcelTrackerServiceTests
    {
        private const string ValidBarcode = "RMARS1234567890123456789M";
        private const string AnotherValidBarcode = "RMARS0987654321098765432Z";
        private const string YetAnotherValidBarcode = "RMARS2749567620571938574Q";
        private const string InvalidBarcode = "RMARS12345678901234567890";

        private static (DeliveryService deliveryService, DateTime? launchDate, int? etaDays) PrepareStandardDeliveryValues(DateTime utcNow)
        {
            var deliveryService = DeliveryService.Standard;

            var (launchDate, etaDays) = deliveryService.ProcessSelectedDeliveryService(utcNow);

            return (deliveryService, launchDate, etaDays);
        }

        private static (DeliveryService deliveryService, DateTime? launchDate, int? etaDays) PrepareExpressDeliveryValues(DateTime utcNow)
        {
            var deliveryService = DeliveryService.Express;

            var (launchDate, etaDays) = deliveryService.ProcessSelectedDeliveryService(utcNow);

            return (deliveryService, launchDate, etaDays);
        }

        private static Parcel CreateParcel(string barcode, DeliveryService deliveryService, int etaDays, DateTime launchDate, ParcelStatus parcelStatus)
        {
            return new Parcel
            {
                Barcode = barcode,
                Contents = "Signed C# language specification and a Christmas card",
                DeliveryService = deliveryService,
                Destination = "New London",
                EstimatedArrivalDate = launchDate.AddDays(etaDays),
                EtaDays = etaDays,
                LaunchDate = launchDate,
                Origin = "Starport Thames Estuary",
                Recipient = "Elon Musk",
                Sender = "Anders Hejlsberg",
                Status = parcelStatus
            };
        }

        private static Mock<IDbContextFactory<ParcelTrackingDb>> PrepareDbFactory()
        {
            var dbOptions = new DbContextOptionsBuilder<ParcelTrackingDb>()
                .UseInMemoryDatabase("MarsParcelTrackingDB")
                .Options;

            var dbContext = new ParcelTrackingDb(dbOptions);

            var dbFactory = new Mock<IDbContextFactory<ParcelTrackingDb>>();

            dbFactory
                .Setup(p => p.CreateDbContextAsync(CancellationToken.None))
                .Returns(Task.FromResult(dbContext));

            return dbFactory;
        }

        private static async Task<Mock<IDbContextFactory<ParcelTrackingDb>>> PrepareDbFactoryForDataWithParcelStatusOfLandedOnMars(Parcel parcel, DateTime utcNow)
        {
            var dbOptions = new DbContextOptionsBuilder<ParcelTrackingDb>()
                .UseInMemoryDatabase("MarsParcelTrackingDB")
                .Options;

            var dbContext = new ParcelTrackingDb(dbOptions);

            dbContext.Parcels.Add(parcel);

            var parcelHistories = new List<ParcelHistory>
            {
                new() {
                    ParcelID = parcel.Id,
                    Status = ParcelStatus.Created,
                    Timestamp = utcNow
                },
                new() {
                    ParcelID = parcel.Id,
                    Status = ParcelStatus.OnRocketToMars,
                    Timestamp = parcel.LaunchDate
                },
                new()
                {
                    ParcelID = parcel.Id,
                    Status = ParcelStatus.LandedOnMars,
                    Timestamp = parcel.EstimatedArrivalDate
                }
            };

            dbContext.ParcelHistories.AddRange(parcelHistories);

            await dbContext.SaveChangesAsync();

            var dbFactory = new Mock<IDbContextFactory<ParcelTrackingDb>>();

            dbFactory
                .Setup(p => p.CreateDbContextAsync(CancellationToken.None))
                .Returns(Task.FromResult(dbContext));

            return dbFactory;
        }

        private static async Task<Mock<IDbContextFactory<ParcelTrackingDb>>> PrepareDbFactoryForDataWithParcelStatusOfOnRocketToMars(Parcel parcel, DateTime utcNow)
        {
            var dbOptions = new DbContextOptionsBuilder<ParcelTrackingDb>()
                .UseInMemoryDatabase("MarsParcelTrackingDB")
                .Options;

            var dbContext = new ParcelTrackingDb(dbOptions);

            dbContext.Parcels.Add(parcel);

            var parcelHistories = new List<ParcelHistory>
            {
                new() {
                    ParcelID = parcel.Id,
                    Status = ParcelStatus.Created,
                    Timestamp = utcNow
                },
                new() {
                    ParcelID = parcel.Id,
                    Status = ParcelStatus.OnRocketToMars,
                    Timestamp = utcNow.AddDays(1)
                }
            };

            dbContext.ParcelHistories.AddRange(parcelHistories);

            await dbContext.SaveChangesAsync();

            var dbFactory = new Mock<IDbContextFactory<ParcelTrackingDb>>();

            dbFactory
                .Setup(p => p.CreateDbContextAsync(CancellationToken.None))
                .Returns(Task.FromResult(dbContext));

            return dbFactory;
        }

        private static async Task<Mock<IDbContextFactory<ParcelTrackingDb>>> PrepareDbFactoryAndData(Parcel parcel, DateTime utcNow)
        {
            var dbOptions = new DbContextOptionsBuilder<ParcelTrackingDb>()
                .UseInMemoryDatabase("MarsParcelTrackingDB")
                .Options;

            var dbContext = new ParcelTrackingDb(dbOptions);

            dbContext.Parcels.Add(parcel);

            var parcelHistory = new ParcelHistory
            {
                ParcelID = parcel.Id,
                Status = parcel.Status,
                Timestamp = utcNow
            };
            
            dbContext.ParcelHistories.Add(parcelHistory);

            await dbContext.SaveChangesAsync();

            var dbFactory = new Mock<IDbContextFactory<ParcelTrackingDb>>();

            dbFactory
                .Setup(p => p.CreateDbContextAsync(CancellationToken.None))
                .Returns(Task.FromResult(dbContext));

            return dbFactory;
        }

        private static ParcelPostRequestDTO PrepareInvalidParcelPostRequestData(Parcel parcel)
        {
            return new ParcelPostRequestDTO
            {
                Barcode = parcel.Barcode,
                Contents = parcel.Contents,
                DeliveryService = parcel.DeliveryService,
                Recipient = parcel.Recipient,
                Sender = parcel.Sender
            };
        }

        private static ParcelPostRequestDTO PrepareValidParcelPostRequestData(DeliveryService deliveryService, string barcode)
        {
            return new ParcelPostRequestDTO
            {
                Barcode = barcode,
                Contents = "Signed C# language specification and a Christmas card",
                DeliveryService = deliveryService,
                Recipient = "Elon Musk",
                Sender = "Anders Hejlsberg"
            };
        }

        [Fact]
        public async Task CreateStandardParcelDelivery_WasSuccessful()
        {
            var utcNow = DateTime.UtcNow;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareStandardDeliveryValues(utcNow);

            var parcel = CreateParcel(ValidBarcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.Created);

            var dbFactory = await PrepareDbFactoryAndData(parcel, utcNow);

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            var (responseDTO, errorMessage) = await service.CreateNewParcelAsync(PrepareValidParcelPostRequestData(deliveryService, ValidBarcode));

            Assert.Empty(errorMessage);
            Assert.NotNull(responseDTO);
        }

        [Fact]
        public async Task CreateExpressParcel_WasSuccessful()
        {
            var utcNow = DateTime.UtcNow;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareExpressDeliveryValues(utcNow);

            var parcel = CreateParcel(ValidBarcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.Created);

            var dbFactory = PrepareDbFactory();

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            var (responseDTO, errorMessage) = await service.CreateNewParcelAsync(PrepareValidParcelPostRequestData(deliveryService, ValidBarcode));

            Assert.Empty(errorMessage);
            Assert.NotNull(responseDTO);
        }

        [Fact]
        public async Task CreateExpressParcel_Failure_InvalidBarcode()
        {
            var utcNow = DateTime.UtcNow;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareExpressDeliveryValues(utcNow);

            var parcel = CreateParcel(InvalidBarcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.Created);

            var dbFactory = PrepareDbFactory();

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            var (responseDTO, errorMessage) = await service.CreateNewParcelAsync(PrepareInvalidParcelPostRequestData(parcel));

            Assert.NotEmpty(errorMessage);
            Assert.Null(responseDTO);
        }

        [Fact]
        public async Task GetParcelDetails_WasSuccessful()
        {
            var utcNow = DateTime.UtcNow;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareExpressDeliveryValues(utcNow);

            var parcel = CreateParcel(YetAnotherValidBarcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.LandedOnMars);

            var dbFactory = await PrepareDbFactoryForDataWithParcelStatusOfLandedOnMars(parcel, utcNow);

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            var (responseDTO, errorMessage) = await service.GetParcelDetailsAsync(YetAnotherValidBarcode);

            Assert.Empty(errorMessage);
            Assert.NotNull(responseDTO);
            Assert.NotNull(responseDTO.History?.FirstOrDefault(w => w.Status == Enum.GetName(ParcelStatus.Created)));
            Assert.NotNull(responseDTO.History?.FirstOrDefault(w => w.Status == Enum.GetName(ParcelStatus.OnRocketToMars)));
            Assert.NotNull(responseDTO.History?.FirstOrDefault(w => w.Status == Enum.GetName(ParcelStatus.LandedOnMars)));
        }

        [Fact]
        public async Task UpdateParcelStatusToLandedOnMars_WasSuccessful()
        {
            var utcNow = DateTime.UtcNow;
            var barcode = ValidBarcode;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareStandardDeliveryValues(utcNow);

            var parcel = CreateParcel(ValidBarcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.OnRocketToMars);

            var dbFactory = await PrepareDbFactoryForDataWithParcelStatusOfOnRocketToMars(parcel, utcNow);

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            clock.Setup(p => p.UtcNow)
                .Returns(parcel.EstimatedArrivalDate.AddDays(1).ToUniversalTime());

            var onLandedOnMarsUpdateResult = await service.UpdateParcelStatusAsync(
                barcode,
                new ParcelPatchRequestDTO
                {
                    NewStatus = Enum.GetName(ParcelStatus.LandedOnMars)
                }
            );

            Assert.Empty(onLandedOnMarsUpdateResult);
        }

        [Fact]
        public async Task UpdateParcelStatusToLandedOnMars_WasInvalidResult_StillInTransit()
        {
            var utcNow = DateTime.UtcNow;
            var barcode = ValidBarcode;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareStandardDeliveryValues(utcNow);

            var parcel = CreateParcel(ValidBarcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.OnRocketToMars);

            var dbFactory = await PrepareDbFactoryForDataWithParcelStatusOfOnRocketToMars(parcel, utcNow);

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            var onLandedOnMarsUpdateResult = await service.UpdateParcelStatusAsync(
                barcode,
                new ParcelPatchRequestDTO
                {
                    NewStatus = Enum.GetName(ParcelStatus.LandedOnMars)
                }
            );

            Assert.NotEmpty(onLandedOnMarsUpdateResult);
            Assert.Contains("Estimated Arrival Date", onLandedOnMarsUpdateResult);
            Assert.Contains("is in the future", onLandedOnMarsUpdateResult);
        }

        [Fact]
        public async Task UpdateExpressParcel_WasSuccessful()
        {
            var utcNow = DateTime.UtcNow;
            var barcode = AnotherValidBarcode;

            var clock = new Mock<IClock>();
            clock.Setup(p => p.UtcNow)
                .Returns(utcNow);

            var (deliveryService, launchDate, etaDays) = PrepareExpressDeliveryValues(utcNow);

            var parcel = CreateParcel(barcode, deliveryService, etaDays.Value, launchDate.Value, ParcelStatus.Created);

            var dbFactory = await PrepareDbFactoryAndData(parcel, utcNow);

            var service = new ParcelTrackingService(clock.Object, dbFactory.Object);

            var etaDate = launchDate.Value.AddDays(etaDays.Value);

            clock.Setup(p => p.UtcNow)
                .Returns(etaDate.ToUniversalTime());

            var result = await service.UpdateParcelStatusAsync(
                barcode,
                new ParcelPatchRequestDTO
                {
                    NewStatus = Enum.GetName(ParcelStatus.OnRocketToMars)
                }
            );

            Assert.Empty(result);
        }
    }
}
