using Serilog;

namespace Penny;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: Directory.GetCurrentDirectory())
            .AddJsonFile(path: "appsettings.json", optional: true)
            .AddJsonFile(path: $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            _ = builder.Host.UseSerilog();

            WebApplication app = builder.Build();
            _ = app.MapGet("/", () => "Hello World!");

            await app.RunAsync();

            Log.Information("Application finished normally");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
