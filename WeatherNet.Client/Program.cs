using WeatherNet.Client.Interfaces;
using WeatherNet.Client.Services;

// Задание 2. Авторизация по JWT токену через gRPC
IAuthService authService = new AuthService();

var response = await authService.GetSecret();
Console.WriteLine(response);

// Задание 1. Стриминг погоды по gRPC
IWeatherService weatherService = new WeatherService();

await weatherService.StartWeatherStream();