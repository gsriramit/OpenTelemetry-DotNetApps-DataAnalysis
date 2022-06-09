
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
using KafkaTriggerApp;

[assembly: FunctionsStartup(typeof(Startup))]
namespace KafkaTriggerApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string serviceName = "opentelemetry-listener-service";
            // OpenTelemetry Resource to be associated with logs, metrics and traces
            var openTelemetryResourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);
            // Enable Tracing with OpenTelemetry
            var openTelemetryTracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource(serviceName)
                .SetResourceBuilder(openTelemetryResourceBuilder)
                .SetSampler(new AlwaysOnSampler())
                //.AddAspNetCoreInstrumentation()
                //.AddHttpClientInstrumentation()
                //.AddSqlClientInstrumentation()
                .AddConsoleExporter()
                .Build();
            builder.Services.AddSingleton(openTelemetryTracerProvider);
            // Optionally inject the service-level tracer
            builder.Services.AddSingleton(openTelemetryTracerProvider.GetTracer(serviceName));


        }
    }
}
