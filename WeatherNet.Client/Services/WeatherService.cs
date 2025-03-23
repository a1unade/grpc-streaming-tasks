using Grpc.Core;
using Grpc.Net.Client;
using WeatherNet.Client.Interfaces;

namespace WeatherNet.Client.Services;

public class WeatherService: IWeatherService
{
    private readonly Weather.WeatherClient _client;

    public WeatherService()
    {
        var serverAddress = "http://localhost:5000";

        var channel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
        {
            HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        });

        _client = new Weather.WeatherClient(channel);
    }

    public async Task StartWeatherStream()
    {
        var request = new WeatherRequest { Longitude = 49.108891, Latitude = 55.796391 };

        using var streamingCall = _client.GetWeatherUpdates(request);

        try
        {
            await foreach (var update in streamingCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{update.RequestTime} погода в Казани на {update.Timestamp} = {update.Temperature}");
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            Console.WriteLine("Weather updates cancelled.");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Status}");
        }
    }
}