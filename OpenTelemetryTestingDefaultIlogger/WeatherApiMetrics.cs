using System.Diagnostics.Metrics;

namespace OpenTelemetryTestingDefaultIlogger;

public class WeatherApiMetrics
{
	public const string ServiceName = "WeatherApi";

    private Counter<int> WeatherApiCounter { get; }

    public WeatherApiMetrics(IMeterFactory meterFactory)
	{
		var meter = meterFactory.Create(ServiceName);

		WeatherApiCounter = meter.CreateCounter<int>("weatherapi-call-count", "Call");
	}

	public void AddWeatherApiCall() => WeatherApiCounter.Add(1);
}
