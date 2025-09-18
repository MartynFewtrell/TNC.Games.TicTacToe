using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Tnc.Games.TicTacToe.Api.Telemetry;

public static class TelemetryExtensions
{
    public const string SourceName = "Tnc.Games.TicTacToe.Api";
    private const string MeterName = "Tnc.Games.TicTacToe.Api.Metrics";

    public static IServiceCollection AddAppTelemetry(this IServiceCollection services)
    {
        // Minimal telemetry wiring: register an ActivitySource and a Meter for manual instrumentation.
        var activitySource = new ActivitySource(SourceName);
        services.AddSingleton(activitySource);

        var meter = new Meter(MeterName);
        services.AddSingleton(meter);

        // Note: full OpenTelemetry registration (exporters/instrumentation) can be added later when packages and policies allow.
        return services;
    }
}
