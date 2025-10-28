
using FluentValidation;
using RMParcelTracker.Api.Common;
using RMParcelTracker.Api.Common.Repository;
using RMParcelTracker.Api.Features.Parcel.Register;
using RMParcelTracker.Api.Features.Parcel.Register.Validators;
using RMParcelTracker.Api.Features.Parcel.Update;
using SystemClock = RMParcelTracker.Api.Common.SystemClock;

namespace RMParcelTracker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterParcelRequestValidator>();
            builder.Services.AddScoped<RegisterParcel>();
            builder.Services.AddScoped<UpdateParcelStatus>();
            builder.Services.AddScoped<IClock, SystemClock>();
            builder.Services.AddSingleton<ParcelRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
           
            app.UseSwagger(); 
            app.UseSwaggerUI();
            

            app.UseHttpsRedirection();
      
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
