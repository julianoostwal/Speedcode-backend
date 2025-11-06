using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using speedCode.security;
using Xunit;

namespace speedCode.Tests.WsRoom;

public class WsRoomTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WsRoomTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // 直接添加 user-secrets
                config.AddUserSecrets<Program>();

                // 读取配置
                var configuration = config.Build();
                var jwtSecret = configuration["SupabaseAuth:JwtSecret"];
                var authority = configuration["SupabaseAuth:Authority"];
                var metadata = configuration["SupabaseAuth:MetadataAddress"];

                if (string.IsNullOrWhiteSpace(jwtSecret))
                    throw new InvalidOperationException("Missing SupabaseAuth:JwtSecret in configuration for test.");

                AuthManager.SetJwtSecret(jwtSecret);
            });
        });
    }

    [Fact]
    public async Task testCreateRoom()
    {
        var jwtSecret = _factory.Services.GetRequiredService<IConfiguration>()["SupabaseAuth:JwtSecret"];
        AuthManager.SetJwtSecret(jwtSecret);
        var client = _factory.Server.CreateWebSocketClient();
        var roomId = Guid.Parse("f528d4d4-94ae-428f-abec-9a8ae087896e");
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var token = AuthManager.GenerateJwtToken("00e96d9a-c738-46ba-9c77-900ab3e17824");
        var message = $@"{{""type"":""create"",""token"":""{token}"",""roomId"":""{roomId}""}}";


        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        Assert.Equal(WebSocketState.Open, ws.State);
    }
}