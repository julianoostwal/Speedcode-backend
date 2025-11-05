using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.exercise;
using speedCode.Exercise.service;
using speedCode.Modularity;

namespace speedCode.Exercise;

public class ExerciseModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ExerciseDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<ExerciseService>();
    }
}