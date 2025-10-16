using Microsoft.Extensions.DependencyInjection;

namespace speedCode.Core.interfaces;

/// <summary>
/// 所有模块都实现此接口，用于统一注册服务和控制器
/// </summary>
public interface IModule
{
    /// <summary>
    /// dependency injection
    /// </summary>
    /// <param name="services">DI services</param>
    void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// some thing wil happend before initalizeModule like seeder
    /// </summary>
    void InitializeModule();
}