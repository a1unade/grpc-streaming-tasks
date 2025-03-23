using System.Runtime.Serialization;
using System.Text.Json;
using Grpc.Core;
using WeatherNet.Common.Contracts;

namespace WeatherNet.DataSender.Services;

public class WeatherGrpcService(HttpClient httpClient, IConfiguration configuration): Weather.WeatherBase
{
    private readonly string _resourceUrl = configuration["WeatherApiConfig:ResourceUri"]!;
    
    public override async Task GetWeatherUpdates(WeatherRequest request,
        IServerStreamWriter<WeatherResponse> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            WeatherResponse apiResponse = await FetchWeatherUpdates(request.Latitude, request.Longitude);
            await responseStream.WriteAsync(apiResponse);
            
            await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
        }
    }

    /// <summary>
    /// Запрос данных с API
    /// </summary>
    /// <param name="latitude">Широта</param>
    /// <param name="longitude">Долгота</param>
    /// <returns>Возвращаем текущую температуру по координатам</returns>
    /// <exception cref="SerializationException">Ошибка десериализации, если ответ пришел, но данных нет</exception>
    private async Task<WeatherResponse> FetchWeatherUpdates(double latitude, double longitude)
    {
        string url = $"{_resourceUrl}?latitude={latitude}&longitude={longitude}&current=temperature_2m";
        
        string json = await httpClient.GetStringAsync(url);
    
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("current", out JsonElement current))
        {
            WeatherApiResponse? apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(current.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse is null)
                throw new SerializationException("Failed to deserialize weather data");

            return new WeatherResponse
            {
                RequestTime = DateTime.UtcNow.ToString("HH:mm:ss"),
                Temperature = apiResponse.Temperature,
                Timestamp = apiResponse.Time.Split('T')[0]
            };
        }

        throw new SerializationException("Failed to fetch weather data");
    }
}