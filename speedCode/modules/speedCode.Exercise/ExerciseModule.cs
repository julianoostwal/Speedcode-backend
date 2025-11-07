using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Core.interfaces;
using speedCode.exercise;
using speedCode.Exercise.Interfaces;
using speedCode.Exercise.repository;
using speedCode.Exercise.service;
using speedCode.Modularity;

namespace speedCode.Exercise;

public class ExerciseModule : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ExerciseDb>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ExerciseService>();
        services.AddScoped<IExerciseQueryService, ExerciseQueryService>();
        services.AddScoped<IExerciseTestService, ExerciseTestService>();
    }
}