namespace speedCode.Modularity;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ModuleLoader
{
    public static void RegisterModules(IServiceCollection services, IConfiguration configuration)
    {
        var modules = LoadModules();

        foreach (var module in modules)
        {
            module.RegisterModule(services, configuration);
        }

        services.AddSingleton(modules);
    }

    // public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    // {
    //     var modules = endpoints.ServiceProvider.GetRequiredService<IEnumerable<IModule>>();
    //     foreach (var module in modules)
    //     {
    //         module.MapEndpoints(endpoints);
    //     }
    // }

    private static IEnumerable<IModule> LoadModules()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        var loadedNames = assemblies.Select(a => a.GetName().Name).ToHashSet();
        var referenced = assemblies
            .SelectMany(a => a.GetReferencedAssemblies())
            .Where(a => !loadedNames.Contains(a.Name))
            .ToList();

        foreach (var asmName in referenced)
        {
            try
            {
                var asm = AppDomain.CurrentDomain.Load(asmName);
                assemblies.Add(asm);
            }
            catch
            {
            }
        }

        var moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        foreach (var type in moduleTypes)
        {
            if (Activator.CreateInstance(type) is IModule module)
                yield return module;
        }
    }
}