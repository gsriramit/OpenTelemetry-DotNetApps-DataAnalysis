using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using DatabaseTraceAnalysis.DataModels;
using System.Diagnostics;
using Microsoft.Azure.Cosmos.Diagnostics;
using OpenTelemetry.Trace;

namespace DatabaseTraceAnalysis
{
    public class CosmosClientTraceAnalysisFunction
    {
       
        private readonly Tracer tracer;
        public CosmosClientTraceAnalysisFunction(Tracer _tracer)
        {
            tracer = _tracer;
        }

        [FunctionName("CosmosClientTraceAnalysisFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context,
            ILogger log)
        {   
            // Create the OTEL Parent Span
            using var parentSpan = tracer.StartActiveSpan("func-httptrigger-span", SpanKind.Server);
            var spanContext = parentSpan.Context;
            //perform a basic read operation 
            try
            {
                // set the span attributes that describe the executed operation
                parentSpan.SetAttribute("faas.trigger", "http");
                parentSpan.SetAttribute("faas.execution", context.InvocationId.ToString());
                parentSpan.SetAttribute("faas.name", context.FunctionName);

                parentSpan.SetAttribute("http.method", req.Method);
                parentSpan.SetAttribute("http.http.host", req.Host.Value);
                parentSpan.SetAttribute("http.scheme", req.Scheme);
                parentSpan.SetAttribute("http.server_name", req.Host.Host);
                parentSpan.SetAttribute("net.host.port", req.Host.Port.Value);
                parentSpan.SetAttribute("http.route", req.Path);

                DateTime invocationStartTime = DateTime.UtcNow;
                parentSpan.SetAttribute("StartTime", invocationStartTime.ToLongTimeString());
                ItemResponse<CustomerProfile> entityResponse = await ReadCustomerData();
                DateTime invocationCompletionTime = DateTime.UtcNow;
                parentSpan.SetAttribute("EndTime", invocationCompletionTime.ToLongTimeString());             
                // calculate the total duration of the cosmos read operation
                double elapsedTime = (invocationCompletionTime - invocationCompletionTime).TotalMilliseconds;              
                // set the status of the http request
                parentSpan.SetAttribute("http.status_code", entityResponse.StatusCode == System.Net.HttpStatusCode.OK? "200":"500");
            }
            catch (Exception ex)
            {
                parentSpan.RecordException(ex);
                
            }
            parentSpan.End();
            string responseMessage = "This HTTP triggered function executed successfully";
            return new OkObjectResult(responseMessage);
        }

        private async Task<ItemResponse<CustomerProfile>> ReadCustomerData()
        {
            using var childSpan = tracer.StartActiveSpan("cosmos-span", SpanKind.Internal);

            var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("cosmosConnectionString"));
            var container = cosmosClient.GetContainer(EDA_DATABASE_ID, EDA_CONTAINER_ID);
            // Assign the variables needed for the document lookup. Note: Clean this up to take only dynamic values
            string itemId = "c98d7693-eb02-486a-8961-9ae8e8fde83a";
            string customerId = "AW00011001";
            DateTime invocationStartTime = DateTime.UtcNow;
            ItemResponse<CustomerProfile> entityResponse = await container.ReadItemAsync<CustomerProfile>(itemId, new PartitionKey(customerId));
            DateTime invocationCompletionTime = DateTime.UtcNow;
            // calculate the total duration of the cosmos read operation
            double elapsedTime = (invocationCompletionTime - invocationCompletionTime).TotalMilliseconds;
            // set the span attributes that describe the executed operation
            childSpan.SetAttribute("DisplayName", COSMOS_SERVER_NAME);
            childSpan.SetAttribute("StartTime", invocationStartTime.ToLongTimeString());
            childSpan.SetAttribute("EndTime", invocationCompletionTime.ToLongTimeString());
            childSpan.SetAttribute("db.system", "cosmosdb");
            childSpan.SetAttribute("db.name", EDA_DATABASE_ID);
            childSpan.SetAttribute("db.cosmosdb.collection", EDA_CONTAINER_ID);
            childSpan.SetStatus(entityResponse.StatusCode == System.Net.HttpStatusCode.OK ? Status.Ok : Status.Error);

            return entityResponse;
        }

        private const string EDA_DATABASE_ID = "db.customer";
        private const string EDA_CONTAINER_ID = "customer.state";
        private const string COSMOS_SERVER_NAME = "cosmos-saga-eda.documents.azure.com";
        private const string OTEL_COSMOS_SYSTEM_NAME = "cosmosdb";

    }
}
