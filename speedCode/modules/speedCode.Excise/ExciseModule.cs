using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Modularity;
using speedCode.services;

namespace speedCode.Excise;

public class ExciseModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ExciseDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<RoomWebSocketHandler>();
        services.AddScoped<ExciseService>();
        services.AddScoped<RoomValidator>();
    }
}