using Moq;
using speedCode.Core.interfaces;
using speedCode.rooms;
using speedCode.WsRoom;
using Xunit;

namespace speedCode.Tests.WsRoom;

public class RoomValidatorTests
{
    private readonly Mock<IRoomValidationService> _mockValidationService;
    private readonly Mock<IProfileQueryService> _mockProfileQueryService;
    private readonly RoomValidator _roomValidator;

    public RoomValidatorTests()
    {
        _mockValidationService = new Mock<IRoomValidationService>();
        _mockProfileQueryService = new Mock<IProfileQueryService>();
        _roomValidator = new RoomValidator(_mockValidationService.Object, _mockProfileQueryService.Object);
    }

    [Fact]
    public async Task RoomExistsAsync_WhenRoomExists_ReturnsTrue()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _mockValidationService.Setup(x => x.RoomExistsAsync(roomId))
            .ReturnsAsync(true);

        // Act
        var result = await _roomValidator.RoomExistsAsync(roomId);

        // Assert
        Assert.True(result);
        _mockValidationService.Verify(x => x.RoomExistsAsync(roomId), Times.Once);
    }

    [Fact]
    public async Task RoomExistsAsync_WhenRoomDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _mockValidationService.Setup(x => x.RoomExistsAsync(roomId))
            .ReturnsAsync(false);

        // Act
        var result = await _roomValidator.RoomExistsAsync(roomId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ClientExistsAsync_WhenClientExists_ReturnsTrue()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var profileDto = new ProfileDto
        {
            id = clientId,
            nickName = "testuser",
            avatarUrl = "https://example.com/avatar.png"
        };
        
        _mockProfileQueryService.Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync(profileDto);

        // Act
        var result = await _roomValidator.ClientExistsAsync(clientId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ClientExistsAsync_WhenClientDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        _mockProfileQueryService.Setup(x => x.GetByIdAsync(clientId))
            .ReturnsAsync((ProfileDto?)null);

        // Act
        var result = await _roomValidator.ClientExistsAsync(clientId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsRoomStarted_WhenRoomExists_ReturnsCorrectState()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        
        // Create a room in WSRoomsService
        WSRoomsService.CreateRoom(roomId, creatorId);

        // Act
        var result = _roomValidator.IsRoomStarted(roomId);

        // Assert
        Assert.False(result); // Room should not be started initially
        
        // Cleanup
        WSRoomsService.Rooms.TryRemove(roomId, out _);
    }

    [Fact]
    public void IsRoomStarted_WhenRoomDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentRoomId = Guid.NewGuid();

        // Act
        var result = _roomValidator.IsRoomStarted(nonExistentRoomId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ClientAllowToJoin_WhenAllowed_ReturnsTrue()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _mockValidationService.Setup(x => x.ClientAllowToJoinAsync(roomId, clientId))
            .ReturnsAsync(true);

        // Act
        var result = await _roomValidator.ClientAllowToJoin(roomId, clientId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ClientAllowToJoin_WhenNotAllowed_ReturnsFalse()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _mockValidationService.Setup(x => x.ClientAllowToJoinAsync(roomId, clientId))
            .ReturnsAsync(false);

        // Act
        var result = await _roomValidator.ClientAllowToJoin(roomId, clientId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRoomAdmin_WhenUserIsAdmin_ReturnsTrue()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _mockValidationService.Setup(x => x.IsRoomAdminAsync(roomId, clientId))
            .ReturnsAsync(true);

        // Act
        var result = await _roomValidator.isRoomAdmin(roomId, clientId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsRoomAdmin_WhenUserIsNotAdmin_ReturnsFalse()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _mockValidationService.Setup(x => x.IsRoomAdminAsync(roomId, clientId))
            .ReturnsAsync(false);

        // Act
        var result = await _roomValidator.isRoomAdmin(roomId, clientId);

        // Assert
        Assert.False(result);
    }
}
