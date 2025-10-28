using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Excise;
using speedCode.Modularity;

namespace speedCode.WsRoom;

public class WsRoomModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RoomWebSocketHandler>();
        services.AddScoped<RoomValidator>();
    }
}