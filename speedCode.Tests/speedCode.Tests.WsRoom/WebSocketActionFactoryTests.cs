using speedCode.WsRoom;
using speedCode.WsRoom.handlers;
using Moq;
using Xunit;

namespace speedCode.Tests.WsRoom;

public class WebSocketActionFactoryTests
{
    [Fact]
    public void GetHandler_WithJoinAction_ReturnsJoinHandler()
    {
        // Arrange
        var mockJoinHandler = new Mock<JoinHandler>(null, null, null);
        var mockStartHandler = new Mock<StartRoomHandler>(null);
        
        var factory = new WebSocketActionFactory(mockJoinHandler.Object, mockStartHandler.Object);

        // Act
        var handler = factory.GetHandler("join");

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<JoinHandler>(handler);
    }

    [Fact]
    public void GetHandler_WithStartAction_ReturnsStartRoomHandler()
    {
        // Arrange
        var mockJoinHandler = new Mock<JoinHandler>(null, null, null);
        var mockStartHandler = new Mock<StartRoomHandler>(null);
        
        var factory = new WebSocketActionFactory(mockJoinHandler.Object, mockStartHandler.Object);

        // Act
        var handler = factory.GetHandler("start");

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<StartRoomHandler>(handler);
    }

    [Fact]
    public void GetHandler_WithCaseInsensitiveAction_ReturnsCorrectHandler()
    {
        // Arrange
        var mockJoinHandler = new Mock<JoinHandler>(null, null, null);
        var mockStartHandler = new Mock<StartRoomHandler>(null);
        
        var factory = new WebSocketActionFactory(mockJoinHandler.Object, mockStartHandler.Object);

        // Act
        var joinHandler = factory.GetHandler("JOIN");
        var startHandler = factory.GetHandler("START");

        // Assert
        Assert.NotNull(joinHandler);
        Assert.NotNull(startHandler);
        Assert.IsType<JoinHandler>(joinHandler);
        Assert.IsType<StartRoomHandler>(startHandler);
    }

    [Fact]
    public void GetHandler_WithInvalidAction_ReturnsNull()
    {
        // Arrange
        var mockJoinHandler = new Mock<JoinHandler>(null, null, null);
        var mockStartHandler = new Mock<StartRoomHandler>(null);
        
        var factory = new WebSocketActionFactory(mockJoinHandler.Object, mockStartHandler.Object);

        // Act
        var handler = factory.GetHandler("invalid_action");

        // Assert
        Assert.Null(handler);
    }

    [Fact]
    public void GetHandler_WithEmptyString_ReturnsNull()
    {
        // Arrange
        var mockJoinHandler = new Mock<JoinHandler>(null, null, null);
        var mockStartHandler = new Mock<StartRoomHandler>(null);
        
        var factory = new WebSocketActionFactory(mockJoinHandler.Object, mockStartHandler.Object);

        // Act
        var handler = factory.GetHandler("");

        // Assert
        Assert.Null(handler);
    }
}

