using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace OpenTelemetryTestingDefaultIlogger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherApiMetrics _metrics;

        private static ActivitySource _activitySource = new ActivitySource(WeatherApiMetrics.ServiceName);
        
        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherApiMetrics metrics)
        {
            _logger = logger;
            _metrics = metrics;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            // ADDED TO GET SOME DATA INTO SEQ
            _logger.LogInformation("Calling WeatherForecast");
            _metrics.AddWeatherApiCall();

            // Isn't this how you create a Span with an innerspan
            using var activity = _activitySource.StartActivity("WeatherForecats_Get");
            using (var innerActivity = _activitySource.StartActivity("WeatherForecats_ExternalApi"))
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/posts");
                var response = await client.GetAsync("/");
                _logger.LogDebug("External call result={StatusCode}", response.StatusCode);
            }
            //

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
