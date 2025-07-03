using System.Text.Json.Serialization;

namespace SampleAPI
{
    public class WeatherForecast
    {
        public DateOnly? Date { get; set; }

        public int? TemperatureC { get; set; }

        [JsonIgnore]
        public int? TemperatureF => TemperatureC.HasValue
     ? 32 + (int)(TemperatureC.Value / 0.5556)
     : null;

        public string? Summary { get; set; }
    }
}
