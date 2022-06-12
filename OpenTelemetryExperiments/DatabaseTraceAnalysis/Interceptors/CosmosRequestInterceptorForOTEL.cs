using Castle.DynamicProxy;
using DatabaseTraceAnalysis.DataModels;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTraceAnalysis.Interceptors
{
    public class CosmosTracingInterceptor : IInterceptor
    {
        private readonly Tracer tracer;
        public CosmosTracingInterceptor(Tracer _tracer)
        {
            tracer = _tracer;
        }
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("Inside the interception method");
            using var childSpan = tracer.StartActiveSpan("cosmos-span", SpanKind.Internal);
            // set the span attributes that describe the executed operation
            // set this as an app-setting
            childSpan.SetAttribute("DisplayName", "cosmos-saga-eda.documents.azure.com");
            childSpan.SetAttribute("db.system", "cosmosdb");
            // in the actual app code, these should be made available from the constants
            childSpan.SetAttribute("db.name", "db.customer");
            childSpan.SetAttribute("db.cosmosdb.collection", "customer.state");
            DateTime invocationStartTime = DateTime.UtcNow;
            childSpan.SetAttribute("StartTime", invocationStartTime.ToLongTimeString());
            invocation.Proceed();
            DateTime invocationCompletionTime = DateTime.UtcNow;
            // calculate the total duration of the cosmos read operation
            double elapsedTime = (invocationCompletionTime - invocationCompletionTime).TotalMilliseconds;
            childSpan.SetAttribute("EndTime", invocationCompletionTime.ToLongTimeString());
        }

        private async Task InterceptAsync(IInvocation invocation)
        {    
        }
    }
}
