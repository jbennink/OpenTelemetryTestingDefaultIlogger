# OpenTelemetryTestingDefaultIlogger

A small sample project using the default Weatherforcast WebApi template. 

I tried to add OpenTelemetry, and send the resulting logs and Traces to Seq (dunning locally in a docker container (see the included docker-compose)


Issues:
1. Even thoug the blogpost [Adding HTTP/protobuf support to OpenTelemetry log ingestion](https://blog.datalust.co/adding-http-protobuf-support-to-opentelemetry-log-ingestion/) seems to state some simple lines that don't require the use of `builder.AddSeq()` but Logs don't com into my seq instance. 
1. I followed the settings mentioned for traces (gobbled together from multiple sources, sorry forgot) but my trace view remains empty. I do see the traces (and metrics) on the Console courtesy of the ConsoleExporter but I have not been able to see Spans (Activities) in Seq. An older blog post (sorry again, forgot the url) did mention something that Seq only regards a Span a Span when a SpanId **and** an @Start (start time) is present. I tried adding that using the SetTag but that did not help either.

What needs to change to my setup to have a minimum viable config/setup to log using OpenTelemetry to Seq?
