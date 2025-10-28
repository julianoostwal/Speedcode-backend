using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using speedCode.Modularity;

namespace speedCode.Judge0;

public class Judge0Module : IModule
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["Judge0:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("Missing configuration: Judge0:BaseUrl");

        services.AddSingleton<Judge0Client>(_ =>
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            return new Judge0Client(httpClient);
        });

        services.AddSingleton<Judge0Handler>();
    }
}