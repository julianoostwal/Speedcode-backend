using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using speedCode.rooms;
using speedCode.security;

namespace speedCode.Tests;

using System.Net.WebSockets;
using System.Text;
using Xunit;

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
                var dict = new Dictionary<string, string?>
                {
                    { "SupabaseAuth:jwtSecret", Environment.GetEnvironmentVariable("SupabaseAuth__jwtSecret") },
                    { "SupabaseAuth:Authority", Environment.GetEnvironmentVariable("SupabaseAuth__Authority") },
                    {
                        "SupabaseAuth:MetadataAddress",
                        Environment.GetEnvironmentVariable("SupabaseAuth__MetadataAddress")
                    }
                };
                config.AddInMemoryCollection(dict);
            });
        });
    }

    [Fact]
    public async Task testJoinRoom()
    {
        var client = _factory.Server.CreateWebSocketClient();
        Guid roomId = Guid.Parse("f528d4d4-94ae-428f-abec-9a8ae087896e");
        using var ws = await client.ConnectAsync(new Uri($@"ws://localhost/ws/{roomId}"),
            CancellationToken.None);
        var token = AuthManager.GenerateJwtToken("00e96d9a-c738-46ba-9c77-900ab3e17824");
        var message = $@"{{""type"":""auth"",""token"":""{token}""}}";
        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

        var buffer = new byte[1024];
        var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        var received = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Assert.NotEmpty(received);
    }
}