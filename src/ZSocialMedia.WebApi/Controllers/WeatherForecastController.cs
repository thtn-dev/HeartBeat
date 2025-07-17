using ZSocialMedia.Infrastructure.Cache;
using Microsoft.AspNetCore.Mvc;

namespace ZSocialMedia.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IDistributedCacheService _cache;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCacheService distributedCacheService)
    {
        _logger = logger;
        _cache = distributedCacheService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Fetching weather forecast data");
        var cacheKey = "WeatherForecastData";
        var cachedData = _cache.GetStringAsync(cacheKey).Result;
        // if cache miss set new data
        if (string.IsNullOrEmpty(cachedData))
        {
            var weatherData = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            _cache.SetStringAsync(cacheKey, System.Text.Json.JsonSerializer.Serialize(weatherData), TimeSpan.FromSeconds(5)).Wait();
            return weatherData;
        }
        return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(cachedData) ?? Enumerable.Empty<WeatherForecast>();
    }
}