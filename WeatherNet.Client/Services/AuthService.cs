using Grpc.Net.Client;
using Auth;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Protected;
using WeatherNet.Client.Interfaces;

namespace WeatherNet.Client.Services;

public class AuthService : IAuthService
{
    private readonly AuthJwtService.AuthJwtServiceClient _authClient;
    
    private readonly ProtectedService.ProtectedServiceClient _secureClient;

    public AuthService()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5000");
        _authClient = new AuthJwtService.AuthJwtServiceClient(channel);
        _secureClient = new ProtectedService.ProtectedServiceClient(channel);
    }

    private async Task<string> GetAccessToken()
    {
        try
        {
            var response = await _authClient.GenerateJwtTokenAsync(new Empty());
            return response.Token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении токена: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task<string> GetSecret()
    {
        try
        {
            string token = await GetAccessToken();

            var headers = new Metadata
            {
                { "Authorization", $"Bearer {token}" }
            };

            var response = await _secureClient.SecureMethodAsync(new Empty(), headers);
            return response.Message;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            return "Неавторизованный доступ";
        }
        catch (Exception ex)
        {
            return $"{ex.Message}";
        }
    }
}
