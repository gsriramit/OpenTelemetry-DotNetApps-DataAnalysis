# OpenTelemetry-DotNetApps-DataAnalysis

## Automatic Instrumentation for a Function with Outbound calls to SQL 
```
Functions:

        OtelSqlTraceAnalysisFunction: [GET] http://localhost:7071/api/OtelSqlTraceAnalysisFunction

For detailed output, run func with --verbose flag.
[2022-06-07T13:10:09.438Z] Host lock lease acquired by instance ID '000000000000000000000000AE2E3E8A'.
Activity.TraceId:          3ca8a919665d723f9bb8f28bcc5fb6fa
Activity.SpanId:           bacd43b43d97e554
Activity.TraceFlags:           Recorded
Activity.ActivitySourceName: OpenTelemetry.SqlClient
Activity.DisplayName: db-OTEL-dev01
Activity.Kind:        Client
Activity.StartTime:   2022-06-07T13:10:15.0187526Z
Activity.Duration:    00:00:00.4030476
Activity.Tags:
    db.system: mssql
    db.name: db-OTEL-dev01
    peer.service: tcp:srv-otel-test.database.windows.net,1433
    db.statement_type: Text
   StatusCode : UNSET
Resource associated with Activity:
    service.name: opentelemetry-service
    service.instance.id: a5b2d9e0-9347-4132-8f99-fda1e31ae2f9
```
       

```

        CosmosClientTraceAnalysisFunction: [GET,POST] http://localhost:7071/api/CosmosClientTraceAnalysisFunction
        
Activity.TraceId:          bd428ad017015b00f2071f52706b2e3c
Activity.SpanId:           70e0b2ad2e300ea2
Activity.TraceFlags:           Recorded
Activity.ParentSpanId:    d0d5a2d3238c7878
Activity.ActivitySourceName: opentelemetry-service
Activity.DisplayName: cosmos-span
Activity.Kind:        Internal
Activity.StartTime:   2022-06-09T05:41:03.5512467Z
Activity.Duration:    00:00:07.0083715
Activity.Tags:
    DisplayName: cosmos-saga-eda.documents.azure.com
    StartTime: 05:41:04
    EndTime: 05:41:10
    db.system: cosmosdb
    db.name: db.customer
    db.cosmosdb.collection: customer.state
   StatusCode : OK
Resource associated with Activity:
    service.name: opentelemetry-service
    service.instance.id: 230db86f-6c7d-4868-bb1b-f9c23b2bf7d0

Activity.TraceId:          bd428ad017015b00f2071f52706b2e3c
Activity.SpanId:           d0d5a2d3238c7878
Activity.TraceFlags:           Recorded
Activity.ActivitySourceName: opentelemetry-service
Activity.DisplayName: func-httptrigger-span
Activity.Kind:        Server
Activity.StartTime:   2022-06-09T05:41:03.4686051Z
Activity.Duration:    00:00:07.1495673
Activity.Tags:
    faas.trigger: http
    faas.execution: d770a2c6-6e38-4c55-83fb-5b8d240563e3
    faas.name: CosmosClientTraceAnalysisFunction
    http.method: GET
    http.http.host: localhost:7071
    http.scheme: http
    http.server_name: localhost
    net.host.port: 7071
    http.route: /api/CosmosClientTraceAnalysisFunction
    StartTime: 05:41:03
    EndTime: 05:41:10
    http.status_code: 200
Resource associated with Activity:
    service.name: opentelemetry-service

```

```
KafkaProducerAnalysisFunction: [GET,POST] http://localhost:7071/api/KafkaProducerAnalysisFunction

Activity.TraceId:          b68ecc95474ad231c8dade1aa3fd5e08
Activity.SpanId:           90d0bf0d611536e6
Activity.TraceFlags:           Recorded
Activity.ParentSpanId:    879c949dd02826be
Activity.ActivitySourceName: opentelemetry-service
Activity.DisplayName: customer-otel send
Activity.Kind:        Producer
Activity.StartTime:   2022-06-09T12:16:30.7497342Z
Activity.Duration:    00:00:05.1899306
Activity.Tags:
    messaging.system: kafka
    messaging.destination: customer-otel
    messaging.destination_kind:  topic
    messaging.protocol: kafka
    messaging.url: pkc-57jzz.southcentralus.azure.confluent.cloud:9092
    messaging.message_id:
    StartTime: 12:16:31
    EndTime: 12:16:35
Resource associated with Activity:
    service.name: opentelemetry-service
    service.instance.id: c0009643-c265-45a2-9e86-bb34bccfd99b

Activity.TraceId:          b68ecc95474ad231c8dade1aa3fd5e08
Activity.SpanId:           879c949dd02826be
Activity.TraceFlags:           Recorded
Activity.ActivitySourceName: opentelemetry-service
Activity.DisplayName: func-httptrigger-span
Activity.Kind:        Server
Activity.StartTime:   2022-06-09T12:16:30.6667082Z
Activity.Duration:    00:00:38.0784000
Activity.Tags:
    faas.trigger: http
    faas.execution: e03e982e-bf2e-4407-b7d5-9fd425cbdaa1
    faas.name: KafkaProducerAnalysisFunction
    http.method: GET
    http.http.host: localhost:7071
    http.scheme: http
    http.server_name: localhost
    net.host.port: 7071
    http.route: /api/KafkaProducerAnalysisFunction
    StartTime: 12:16:30
    EndTime: 12:17:08
Resource associated with Activity:
    service.name: opentelemetry-service
    service.instance.id: c0009643-c265-45a2-9e86-bb34bccfd99b
    
======================
Activity.TraceId:          b68ecc95474ad231c8dade1aa3fd5e08
Activity.SpanId:           9c43c2a6ee28544f
Activity.TraceFlags:           Recorded
Activity.ParentSpanId:    879c949dd02826be
Activity.ActivitySourceName: opentelemetry-listener-service
Activity.DisplayName: customer-otel receive
Activity.Kind:        Consumer
Activity.StartTime:   2022-06-09T12:18:45.4396333Z
Activity.Duration:    00:01:09.1751232
Activity.Tags:
    messaging.system: kafka
    messaging.destination: customer-otel
    messaging.destination_kind:  topic
    messaging.protocol: kafka
    messaging.url: pkc-57jzz.southcentralus.azure.confluent.cloud:9092
    messaging.consumer_id: $Default
Resource associated with Activity:
    service.name: opentelemetry-listener-service
    service.instance.id: 486d21e9-fa6b-4e27-a8e1-0396354e4ff2
