using DatabaseTraceAnalysis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTelemetry.Trace;
using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DatabaseTraceAnalysis
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // OpenTelemetry Resource to be associated with logs, metrics and traces
            var openTelemetryResourceBuilder = ResourceBuilder.CreateDefault().AddService("opentelemetry-service");
            // Enable Tracing with OpenTelemetry
            var openTelemetryTracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(openTelemetryResourceBuilder)
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddConsoleExporter()
                .Build();
            builder.Services.AddSingleton(openTelemetryTracerProvider);

            builder.
        }
    }
}
