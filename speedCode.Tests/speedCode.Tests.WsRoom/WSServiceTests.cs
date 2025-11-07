using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Moq;
using speedCode.Core.interfaces;
using speedCode.rooms.service;
using Xunit;

namespace speedCode.Tests.WsRoom;

public class WSServiceTests
{
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly WSService _wsService;

    public WSServiceTests()
    {
        _mockJwtService = new Mock<IJwtService>();
        _wsService = new WSService(_mockJwtService.Object);
    }

    [Fact]
    public void ValidateAndReturnJsonProperty_WhenPropertyExists_ReturnsValue()
    {
        // Arrange
        var json = JsonDocument.Parse(@"{""type"":""join"",""roomId"":""123""}").RootElement;

        // Act
        var type = _wsService.ValidateAndReturnJsonProperty("type", json);
        var roomId = _wsService.ValidateAndReturnJsonProperty("roomId", json);

        // Assert
        Assert.Equal("join", type);
        Assert.Equal("123", roomId);
    }

    [Fact]
    public void ValidateAndReturnJsonProperty_WhenPropertyMissing_ThrowsException()
    {
        // Arrange
        var json = JsonDocument.Parse(@"{""type"":""join""}").RootElement;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _wsService.ValidateAndReturnJsonProperty("roomId", json));
        
        Assert.Contains("Missing 'roomId' field", exception.Message);
    }

    [Fact]
    public void ValidateAndReturnJsonProperty_WhenPropertyEmpty_ThrowsException()
    {
        // Arrange
        var json = JsonDocument.Parse(@"{""type"":""""}").RootElement;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _wsService.ValidateAndReturnJsonProperty("type", json));
        
        Assert.Contains("Empty 'type' field", exception.Message);
    }

    [Fact]
    public async Task ValidateAndExtractClientIdAsync_WithValidToken_ReturnsClientId()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var token = "valid_token";
        var json = JsonDocument.Parse($@"{{""token"":""{token}""}}").RootElement;
        
        _mockJwtService.Setup(x => x.GetClientIdFromToken(token))
            .Returns(clientId.ToString());

        var mockSocket = new Mock<WebSocket>();
        mockSocket.Setup(s => s.State).Returns(WebSocketState.Open);

        // Act
        var result = await _wsService.ValidateAndExtractClientIdAsync(mockSocket.Object, json);

        // Assert
        Assert.Equal(clientId, result);
        _mockJwtService.Verify(x => x.GetClientIdFromToken(token), Times.Once);
    }

    [Fact]
    public async Task ValidateAndExtractClientIdAsync_WithInvalidToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var token = "invalid_token";
        var json = JsonDocument.Parse($@"{{""token"":""{token}""}}").RootElement;
        
        _mockJwtService.Setup(x => x.GetClientIdFromToken(token))
            .Returns((string)null);

        var mockSocket = new Mock<WebSocket>();
        mockSocket.Setup(s => s.State).Returns(WebSocketState.Open);
        mockSocket.Setup(s => s.SendAsync(
            It.IsAny<ArraySegment<byte>>(),
            It.IsAny<WebSocketMessageType>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockSocket.Setup(s => s.CloseAsync(
            It.IsAny<WebSocketCloseStatus>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _wsService.ValidateAndExtractClientIdAsync(mockSocket.Object, json));
    }

    [Fact]
    public async Task ValidateAndExtractClientIdAsync_WithMissingToken_ThrowsException()
    {
        // Arrange
        var json = JsonDocument.Parse(@"{""type"":""join""}").RootElement;
        var mockSocket = new Mock<WebSocket>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _wsService.ValidateAndExtractClientIdAsync(mockSocket.Object, json));
    }

    [Fact]
    public async Task CloseWithMessageAsync_ShouldSendMessageAndClose()
    {
        // Arrange
        var mockSocket = new Mock<WebSocket>();
        mockSocket.Setup(s => s.State).Returns(WebSocketState.Open);
        mockSocket.Setup(s => s.SendAsync(
            It.IsAny<ArraySegment<byte>>(),
            It.IsAny<WebSocketMessageType>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockSocket.Setup(s => s.CloseAsync(
            It.IsAny<WebSocketCloseStatus>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var message = "Error message";
        var reason = "Test reason";

        // Act
        await WSService.CloseWithMessageAsync(mockSocket.Object, message, reason);

        // Assert
        mockSocket.Verify(s => s.SendAsync(
            It.Is<ArraySegment<byte>>(seg => Encoding.UTF8.GetString(seg.Array, seg.Offset, seg.Count) == message),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None), Times.Once);
        
        mockSocket.Verify(s => s.CloseAsync(
            WebSocketCloseStatus.PolicyViolation,
            reason,
            CancellationToken.None), Times.Once);
    }
}

