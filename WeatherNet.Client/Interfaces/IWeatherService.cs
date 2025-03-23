namespace WeatherNet.Client.Interfaces;

public interface IWeatherService
{
    /// <summary>
    /// Стриминг погоды по gRPC
    /// </summary>
    /// <returns></returns>
    Task StartWeatherStream();
}