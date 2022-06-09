using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTelemetry.Trace;
using Confluent.Kafka;
using System.Text;

namespace DatabaseTraceAnalysis
{
    public class KafkaProducerAnalysisFunction
    {
        private readonly Tracer tracer;
        public KafkaProducerAnalysisFunction(Tracer _tracer)
        {
            tracer = _tracer;
        }

        [FunctionName("KafkaProducerAnalysisFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

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
                var eventDeliveryResult = await PublishMessageToKafkaTopic();
                DateTime invocationCompletionTime = DateTime.UtcNow;
                parentSpan.SetAttribute("EndTime", invocationCompletionTime.ToLongTimeString());
                // calculate the total duration of the cosmos read operation
                double elapsedTime = (invocationCompletionTime - invocationCompletionTime).TotalMilliseconds;
                // set the status of the http request
                //parentSpan.SetAttribute("http.status_code", entityResponse.StatusCode == System.Net.HttpStatusCode.OK ? "200" : "500");
            }
            catch (Exception ex)
            {
                parentSpan.RecordException(ex);

            }
            parentSpan.End();
            string responseMessage = "This HTTP triggered function executed successfully";
            return new OkObjectResult(responseMessage);
        }

        private async Task<DeliveryResult<Confluent.Kafka.Null,string>> PublishMessageToKafkaTopic()
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BootstrapServer"),
                EnableIdempotence = true,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = Environment.GetEnvironmentVariable("ConfluentCloudUserName"),
                SaslPassword = Environment.GetEnvironmentVariable("ConfluentCloudPassword"),
                SaslMechanism = SaslMechanism.Plain
            };

            using var childSpan = tracer.StartActiveSpan($"{Environment.GetEnvironmentVariable("topic")} send", SpanKind.Producer);
            childSpan.SetAttribute("messaging.system", "kafka");
            childSpan.SetAttribute("messaging.destination", Environment.GetEnvironmentVariable("topic"));
            childSpan.SetAttribute("messaging.destination_kind", " topic");
            childSpan.SetAttribute("messaging.protocol", "kafka");
            childSpan.SetAttribute("messaging.url", Environment.GetEnvironmentVariable("BootstrapServer"));
            

            DeliveryResult<Null, string> deliveryResult;
            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                string testPayload =@"{'name':'Adam', 'country':'USA'}";
                // Instantiate the header collection
                var headers = new Headers();
                // this is the point where we add the headers required for OpenTelemetry (Trace and SpanId of the parent)
                string traceId = childSpan.Context.TraceId.ToString();
                string parentSpanId = childSpan.ParentSpanId.ToString();
                string parentSpanTraceFlag = childSpan.Context.TraceFlags.ToString();

                headers.Add("traceId", Encoding.UTF8.GetBytes(traceId));
                headers.Add("parentSpanId", Encoding.UTF8.GetBytes(parentSpanId));
                headers.Add("parentSpanTraceFlag", Encoding.UTF8.GetBytes(parentSpanTraceFlag));

                DateTime invocationStartTime = DateTime.UtcNow;
                deliveryResult = await producer.ProduceAsync(
                            Environment.GetEnvironmentVariable("topic"),
                                new Message<Null, string>
                                {
                                    Value = testPayload,
                                    Headers = headers
                                });
                DateTime invocationCompletionTime = DateTime.UtcNow;
                // calculate the total duration of the kafka send operation
                double elapsedTime = (invocationCompletionTime - invocationCompletionTime).TotalMilliseconds;
                // TBD
                childSpan.SetAttribute("messaging.message_id", "");
                childSpan.SetAttribute("StartTime", invocationStartTime.ToLongTimeString());
                childSpan.SetAttribute("EndTime", invocationCompletionTime.ToLongTimeString());

                return deliveryResult;
            }
        }
    }
}
