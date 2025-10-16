using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Modularity;
using speedCode.Profile;

namespace speedCode.Profile;
public class ProfileModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProfileDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<RoomUserService>();
    }
}