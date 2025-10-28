using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Excise.service;
using speedCode.Modularity;
using speedCode.Profile;

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