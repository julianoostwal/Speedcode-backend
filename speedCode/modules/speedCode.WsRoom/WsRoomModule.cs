using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Excise;
using speedCode.Modularity;
using speedCode.rooms.service;
using speedCode.WsRoom.handlers;

namespace speedCode.WsRoom;

public class WsRoomModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RoomWebSocketHandler>();
        services.AddScoped<RoomValidator>();
        services.AddScoped<WSService>();
        services.AddScoped<JoinHandler>();
        services.AddScoped<WebSocketActionFactory>();
        services.AddScoped<StartRoomHandler>();
        services.AddScoped<CreateHandler>();
        
    }
}