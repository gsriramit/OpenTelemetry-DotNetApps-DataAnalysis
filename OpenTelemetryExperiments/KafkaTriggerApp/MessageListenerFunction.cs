using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Text;

namespace KafkaTriggerApp
{
    public class MessageListenerFunction
    {
        private readonly Tracer tracer;
        public MessageListenerFunction(Tracer _tracer)
        {
            tracer = _tracer;
        }
        // KafkaTrigger sample 
        // Consume the message from "order" on the LocalBroker.
        // Add `BootstrapServer` and `KafkaPassword` to the local.settings.json
        // For EventHubs
        // "BootstrapServer": "{EVENT_HUBS_NAMESPACE}.servicebus.windows.net:9093"
        // "KafkaPassword":"{EVENT_HUBS_CONNECTION_STRING}
        [FunctionName("Function1")]
        public void Run(
            [KafkaTrigger("BootstrapServer",
                          "customer-otel",
                          Username = "ConfluentCloudUserName",
                          Password = "ConfluentCloudPassword",
                          Protocol = BrokerProtocol.SaslSsl,
                          AuthenticationMode = BrokerAuthenticationMode.Plain,
                          ConsumerGroup = "$Default")] KafkaEventData<string>[] events,
            ILogger log)
        {
            string traceId = "";
            string parentSpanId = "";
            string parentSpanTraceFlag = "";
            // sanity check
            if (events!=null && events.Length > 0)
            {
                KafkaEventData<string> kafkaLookupEventData = events[0];
                KafkaEventDataHeaders headers = (KafkaEventDataHeaders)kafkaLookupEventData.Headers;
                traceId = Encoding.UTF8.GetString(headers.GetFirst("traceId"));
                parentSpanId = Encoding.UTF8.GetString(headers.GetFirst("parentSpanId"));
                parentSpanTraceFlag = Encoding.UTF8.GetString(headers.GetFirst("parentSpanTraceFlag"));
            }
            ActivityTraceId parentTraceIdObj = ActivityTraceId.CreateFromString(new ReadOnlySpan<char>(traceId.ToCharArray()));
            ActivitySpanId parentSpanIdObj = ActivitySpanId.CreateFromString(new ReadOnlySpan<char>(parentSpanId.ToCharArray()));
            ActivityTraceFlags activityTraceFlags;
            bool parseResult = Enum.TryParse<ActivityTraceFlags>(parentSpanTraceFlag, out activityTraceFlags);

            using var childSpan = tracer.StartActiveSpan($"{Environment.GetEnvironmentVariable("topic")} receive",
                                                                                                    SpanKind.Consumer, new SpanContext(parentTraceIdObj, parentSpanIdObj, activityTraceFlags));
            childSpan.SetAttribute("messaging.system", "kafka");
            childSpan.SetAttribute("messaging.destination", Environment.GetEnvironmentVariable("topic"));
            childSpan.SetAttribute("messaging.destination_kind", " topic");
            childSpan.SetAttribute("messaging.protocol", "kafka");
            childSpan.SetAttribute("messaging.url", Environment.GetEnvironmentVariable("BootstrapServer"));
            //For Kafka, set it to {messaging.kafka.consumer_group} - {messaging.kafka.client_id}, if both are present, or only messaging.kafka.consumer_group
            childSpan.SetAttribute("messaging.consumer_id", "$Default");

            foreach (KafkaEventData<string> eventData in events)
            {
                log.LogInformation($"C# Kafka trigger function processed a message: {eventData.Value}");
            }
        }
    }
}
