using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Protected;

namespace WeatherNet.DataSender.Services;

[Authorize]
public class ProtectedGrpcService : ProtectedService.ProtectedServiceBase
{
    public override Task<SecureResponse> SecureMethod(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new SecureResponse
        {
            Message = "Секретный секрет!"
        });
    }
}