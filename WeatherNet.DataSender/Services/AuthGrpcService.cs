using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace WeatherNet.DataSender.Services;

public class AuthGrpcService(IConfiguration configuration): AuthJwtService.AuthJwtServiceBase
{
    public override Task<JwtResponse> GenerateJwtToken(Empty request, ServerCallContext context)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: credentials
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new JwtResponse
        {
            Token = tokenValue,
            ExpiresAt = DateTime.UtcNow.AddHours(3).ToString("O")
        });
    }
}