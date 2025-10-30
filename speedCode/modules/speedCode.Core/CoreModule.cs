using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Core.interfaces;
using speedCode.Core.utils;
using speedCode.Modularity;

namespace speedCode.Core;

public class CoreModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var secret = config["SupabaseAuth:JwtSecret"];
            return new JwtService(secret);
        });
    }
}