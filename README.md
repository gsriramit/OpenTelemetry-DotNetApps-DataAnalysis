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