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
using DatabaseTraceAnalysis.Cosmos;
using Microsoft.Azure.Cosmos;
using Castle.DynamicProxy;
using DatabaseTraceAnalysis.Interceptors;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DatabaseTraceAnalysis
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string serviceName = "opentelemetry-service";
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
            var requestTracer = openTelemetryTracerProvider.GetTracer(serviceName);
            builder.Services.AddSingleton(requestTracer);

            // Cosmos Repository - use this to test the method level interception for OTEL
            builder.Services.AddSingleton<ICosmosSqlRepository>((s) =>
            {
                var proxyGenerator = new ProxyGenerator();                
                var cosmosClient = new CosmosClient(cosmosConnectionString);
                return proxyGenerator.CreateInterfaceProxyWithTarget<ICosmosSqlRepository>(
                                    new CosmosSqlRepository(cosmosClient, DATABASE_ID, CONTAINER_ID), 
                                    new CosmosTracingInterceptor(requestTracer));
                
            });
        }

        private string cosmosConnectionString = Environment.GetEnvironmentVariable("cosmosConnectionString");
        private const string DATABASE_ID = "db.customer";
        private const string CONTAINER_ID = "customer.state";
    }
}
