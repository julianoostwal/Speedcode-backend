using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Core.interfaces;
using speedCode.Modularity;
using speedCode.Profile;
using speedCode.Room.Interfaces;
using speedCode.Room.repository;
using speedCode.Room.service;

namespace speedCode.Room;

public class RoomsModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RoomsDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<RoomService>();
        services.AddScoped<RoomUserService>();
        services.AddScoped<IRoomValidationService, RoomValidationService>();
        services.AddScoped<IRoomDataService, RoomDataService>();
    }
}