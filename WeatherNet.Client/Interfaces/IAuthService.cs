namespace WeatherNet.Client.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Получение токена JWT для авторизации по gRPC
    /// </summary>
    /// <returns></returns>
    private Task<string> GetAccessToken()
    {
        return Task.FromResult("");
    }
    
    /// <summary>
    /// Получение секрета по gRPC и авторизации по JWT токену 
    /// </summary>
    /// <returns></returns>
    public Task<string> GetSecret();
}