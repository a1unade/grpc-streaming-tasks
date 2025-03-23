using System.Text.Json.Serialization;

namespace WeatherNet.Common.Contracts;

/// <summary>
/// Ответ из Api
/// </summary>
public class WeatherApiResponse
{
    /// <summary>
    /// Дата и время вместе с тайм-зоной
    /// </summary>
    [JsonPropertyName("time")]
    public required string Time {get; set;}
    
    /// <summary>
    /// Температура в градусах Цельсия
    /// </summary>
    [JsonPropertyName("temperature_2m")]
    public required double Temperature {get; set;}
}