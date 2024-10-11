
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetryTestingDefaultIlogger
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.AddLogging(builder =>
            {
                /* This is supposed to work without AddSeq()  as shown in this post:
                 * https://blog.datalust.co/adding-http-protobuf-support-to-opentelemetry-log-ingestion/
                 * but is dosn't work for logging
                 * 
                builder.AddOpenTelemetry(logging =>
                {
                    logging.SetResourceBuilder(
                        ResourceBuilder.CreateEmpty()
                            .AddService(WeatherApiMetrics.ServiceName)
                            .AddAttributes(new Dictionary<string, object>
                            {
                                ["deployment.environment"] = "development"
                            }));
                    logging.IncludeFormattedMessage = true;
                    logging.IncludeScopes = true;
                    logging.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:5431/ingest/otlp/v1/logs");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    })
                    .AddConsoleExporter(); 
                });
                */
                builder.AddSeq();
            });

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(WeatherApiMetrics.ServiceName))
                .WithMetrics(metrics =>
                {
                    metrics.AddMeter(WeatherApiMetrics.ServiceName);
                    // DISABLED TO REDUCE NOISE
                    //metrics.AddAspNetCoreInstrumentation();
                    //metrics.AddHttpClientInstrumentation();
                    //metrics.AddRuntimeInstrumentation();
                    //metrics.AddProcessInstrumentation();
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:5431/ingest/otlp");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    })
                    .AddConsoleExporter();
                })
                .WithTracing(tracing =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        tracing.SetSampler<AlwaysOnSampler>();
                    }
                    tracing.AddSource(WeatherApiMetrics.ServiceName);
                    // DISABLED TO REDUCE NOISE
                    //tracing.AddAspNetCoreInstrumentation();
                    //tracing.AddHttpClientInstrumentation();
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:5431/ingest/otlp");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    })
                    .AddConsoleExporter();
                });

            builder.Services.AddSingleton<WeatherApiMetrics>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}