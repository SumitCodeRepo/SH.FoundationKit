using Microsoft.AspNetCore.Mvc;
using SH.FoundationKit.Middleware;

namespace SampleAPI.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("get1", Name = "GetWeatherForecast")]

        public IEnumerable<WeatherForecast> Get()
        {
            var forecasts = new[]
{
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        TemperatureC = 25,
        Summary = "Sunny"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
        TemperatureC = 18,
        Summary = "Partly cloudy"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
        TemperatureC = 12,
        Summary = "Rainy"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
        TemperatureC = 30,
        Summary = "Hot"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
        TemperatureC = -2,
        Summary = "Snow"
    }
};
            return forecasts
            .ToArray();
        }


        [HttpGet("EtagTest", Name = "GetWeatherForecast123")]
        [TypeFilter(typeof(EnableETagFilter))]
        public IEnumerable<WeatherForecast> EtagTest()
        {
            var forecasts = new[]
{
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        TemperatureC = 215,
        Summary = "Sunny"
    },
        new WeatherForecast{
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        TemperatureC = 253,
        Summary = "Sunny"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
        TemperatureC = 18,
        Summary = "Partly cloudy"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
        TemperatureC = 12,
        Summary = "Rainy"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
        TemperatureC = 30,
        Summary = "Hot"
    },
    new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
        TemperatureC = -2,
        Summary = "Snow"
    }
};

            HttpContext.Items["__ETagSource__"] = forecasts;
            return forecasts
            .ToArray();
        }

        [HttpGet("get2", Name = "Error")]

        public IEnumerable<WeatherForecast> GetErrror()
        {
            int i = 0;
            int a = 34 / i;
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weather, [FromQuery] int id)
        {
            if (weather == null) return BadRequest("Weather data is required.");

            // Optional: use the id
            return Ok(weather);

        }
    }
}
