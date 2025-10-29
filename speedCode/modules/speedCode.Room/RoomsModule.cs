using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Modularity;
using speedCode.Profile;
using speedCode.Room.service;

namespace speedCode.Room;

public class RoomsModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RoomsDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<RoomService>();
        services.AddScoped<RoomUserService>();
    }
}