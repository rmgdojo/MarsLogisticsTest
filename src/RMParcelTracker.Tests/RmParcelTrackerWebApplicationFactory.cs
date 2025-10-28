using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RMParcelTracker.Api;
using RMParcelTracker.Api.Common;

namespace RMParcelTracker.Tests;

public class RmParcelTrackerWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IClock>(new TestClock(new DateTime(2025, 10, 26)));
        });
    }
    
    public void AdvanceTestClockTime(TimeSpan timespan)
    {
        var clock = Services.GetRequiredService<IClock>() as TestClock;
        clock!.Advance(timespan);
    }
}