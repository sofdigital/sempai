// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SofDigital.Sempai.Extensions;

namespace Sempai.Tests.Features.Fixtures;

/// <summary>
///     Test fixture for ServiceCollection setup.
///     Use with IClassFixture to share the same ServiceProvider instance across all tests in a class.
///     This is more efficient for expensive setup operations.
/// </summary>
public class DependencyFixture : IDisposable
{
    public DependencyFixture()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", true, true);
                config.AddJsonFile("appsettings.Development.json", true, true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, serviceCollection) =>
            {
                Configuration = context.Configuration;

                serviceCollection.AddSempai(Configuration);
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            })
            .Build();

        ServiceProvider = host.Services;
    }

    public IConfiguration? Configuration { get; private set; }

    public IServiceProvider ServiceProvider { get; set; }

    public new void Dispose()
    {
        if (ServiceProvider is IDisposable disposable) disposable.Dispose();

        GC.SuppressFinalize(this);
    }
}