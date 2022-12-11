using OpenTelemetry.Trace;

using Serilog;

namespace Penny;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration: builder.Configuration)
                .CreateLogger();

            _ = builder.Host.UseSerilog();
            _ = builder.Services.AddOpenTelemetryTracing(tpBuilder =>
            {
                _ = tpBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter()
                .AddJaegerExporter();
            });

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
