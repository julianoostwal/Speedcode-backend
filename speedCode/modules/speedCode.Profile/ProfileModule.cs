using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Core.interfaces;
using speedCode.Modularity;
using speedCode.Profile;
using speedCode.Profile.Interfaces;
using speedCode.Profile.repository;
using speedCode.Profile.service;

namespace speedCode.Profile;

public class ProfileModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProfileDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ProfileService>();
        services.AddScoped<IProfileQueryService, ProfileQueryService>();
        services.AddScoped<IProfileCompletionService, ProfileCompletionService>();
    }
}