using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace speedCode.Modularity;

public interface IModule
{
    void RegisterModule(IServiceCollection services, IConfiguration configuration);

}