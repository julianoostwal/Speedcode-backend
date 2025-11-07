using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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
    private readonly string _jwtSecret;

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
        
        _jwtSecret = _factory.Services.GetRequiredService<IConfiguration>()["SupabaseAuth:JwtSecret"];
        AuthManager.SetJwtSecret(_jwtSecret);
    }

    [Fact]
    public async Task TestJoinRoom_ShouldConnectSuccessfully()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var roomId = Guid.NewGuid();
        var userId = "00e96d9a-c738-46ba-9c77-900ab3e17824";
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var token = AuthManager.GenerateJwtToken(userId);
        var message = $@"{{""type"":""join"",""token"":""{token}"",""roomId"":""{roomId}""}}";

        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        
        Assert.Equal(WebSocketState.Open, ws.State);
        
        // Cleanup
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task TestJoinRoom_WithInvalidToken_ShouldClose()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var roomId = Guid.NewGuid();
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var message = $@"{{""type"":""join"",""token"":""invalid_token"",""roomId"":""{roomId}""}}";

        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for close
        var buffer = new byte[1024];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        Assert.True(ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived);
    }

    [Fact]
    public async Task TestStartRoom_ByCreator_ShouldSucceed()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var roomId = Guid.NewGuid();
        var userId = "00e96d9a-c738-46ba-9c77-900ab3e17824";
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var token = AuthManager.GenerateJwtToken(userId);
        
        // First join the room
        var joinMessage = $@"{{""type"":""join"",""token"":""{token}"",""roomId"":""{roomId}""}}";
        var joinBytes = Encoding.UTF8.GetBytes(joinMessage);
        await ws.SendAsync(joinBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        
        // Small delay to ensure join is processed
        await Task.Delay(100);
        
        // Then start the room
        var startMessage = $@"{{""type"":""start"",""token"":""{token}"",""roomId"":""{roomId}""}}";
        var startBytes = Encoding.UTF8.GetBytes(startMessage);
        await ws.SendAsync(startBytes, WebSocketMessageType.Text, true, CancellationToken.None);

        Assert.Equal(WebSocketState.Open, ws.State);
        
        // Cleanup
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task TestJoinRoom_AfterRoomStarted_ShouldBeDenied()
    {
        var roomId = Guid.NewGuid();
        var creatorId = "00e96d9a-c738-46ba-9c77-900ab3e17824";
        var joinerId = "11e96d9a-c738-46ba-9c77-900ab3e17825";
        
        var client1 = _factory.Server.CreateWebSocketClient();
        var client2 = _factory.Server.CreateWebSocketClient();
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        // Creator joins and starts room
        using var ws1 = await client1.ConnectAsync(serverUri, CancellationToken.None);
        var creatorToken = AuthManager.GenerateJwtToken(creatorId);
        
        var joinMessage = $@"{{""type"":""join"",""token"":""{creatorToken}"",""roomId"":""{roomId}""}}";
        await ws1.SendAsync(Encoding.UTF8.GetBytes(joinMessage), WebSocketMessageType.Text, true, CancellationToken.None);
        
        await Task.Delay(100);
        
        var startMessage = $@"{{""type"":""start"",""token"":""{creatorToken}"",""roomId"":""{roomId}""}}";
        await ws1.SendAsync(Encoding.UTF8.GetBytes(startMessage), WebSocketMessageType.Text, true, CancellationToken.None);
        
        await Task.Delay(100);

        // Another user tries to join after room started
        using var ws2 = await client2.ConnectAsync(serverUri, CancellationToken.None);
        var joinerToken = AuthManager.GenerateJwtToken(joinerId);
        
        var joinMessage2 = $@"{{""type"":""join"",""token"":""{joinerToken}"",""roomId"":""{roomId}""}}";
        await ws2.SendAsync(Encoding.UTF8.GetBytes(joinMessage2), WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for response
        var buffer = new byte[1024];
        var result = await ws2.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        // Should be closed or closing
        Assert.True(ws2.State == WebSocketState.Closed || ws2.State == WebSocketState.CloseReceived);
        
        // Cleanup
        await ws1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task TestMultipleClientsJoinRoom_ShouldAllConnect()
    {
        var roomId = Guid.NewGuid();
        var user1Id = "00e96d9a-c738-46ba-9c77-900ab3e17824";
        var user2Id = "11e96d9a-c738-46ba-9c77-900ab3e17825";
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        var client1 = _factory.Server.CreateWebSocketClient();
        var client2 = _factory.Server.CreateWebSocketClient();

        using var ws1 = await client1.ConnectAsync(serverUri, CancellationToken.None);
        using var ws2 = await client2.ConnectAsync(serverUri, CancellationToken.None);

        var token1 = AuthManager.GenerateJwtToken(user1Id);
        var token2 = AuthManager.GenerateJwtToken(user2Id);

        var message1 = $@"{{""type"":""join"",""token"":""{token1}"",""roomId"":""{roomId}""}}";
        var message2 = $@"{{""type"":""join"",""token"":""{token2}"",""roomId"":""{roomId}""}}";

        await ws1.SendAsync(Encoding.UTF8.GetBytes(message1), WebSocketMessageType.Text, true, CancellationToken.None);
        await Task.Delay(100);
        await ws2.SendAsync(Encoding.UTF8.GetBytes(message2), WebSocketMessageType.Text, true, CancellationToken.None);

        Assert.Equal(WebSocketState.Open, ws1.State);
        Assert.Equal(WebSocketState.Open, ws2.State);

        // Cleanup
        await ws1.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
        await ws2.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
    }

    [Fact]
    public async Task TestMissingRoomId_ShouldCloseConnection()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var userId = "00e96d9a-c738-46ba-9c77-900ab3e17824";
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var token = AuthManager.GenerateJwtToken(userId);
        var message = $@"{{""type"":""join"",""token"":""{token}""}}"; // Missing roomId

        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for close
        var buffer = new byte[1024];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        Assert.True(ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived);
    }

    [Fact]
    public async Task TestInvalidMessageType_ShouldCloseConnection()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var roomId = Guid.NewGuid();
        var userId = "00e96d9a-c738-46ba-9c77-900ab3e17824";
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var token = AuthManager.GenerateJwtToken(userId);
        var message = $@"{{""type"":""invalid_action"",""token"":""{token}"",""roomId"":""{roomId}""}}";

        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for close
        var buffer = new byte[1024];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        Assert.True(ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived);
    }

    [Fact]
    public async Task TestEmptyMessage_ShouldCloseConnection()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var message = "";
        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for close
        var buffer = new byte[1024];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        Assert.True(ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived);
    }

    [Fact]
    public async Task TestInvalidJson_ShouldCloseConnection()
    {
        var client = _factory.Server.CreateWebSocketClient();
        var serverUri = new Uri(_factory.Server.BaseAddress, $"/ws");

        using var ws = await client.ConnectAsync(serverUri, CancellationToken.None);

        var message = "{ invalid json }";
        var bytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);

        // Wait for close
        var buffer = new byte[1024];
        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        Assert.True(ws.State == WebSocketState.Closed || ws.State == WebSocketState.CloseReceived);
    }
}