using speedCode.rooms;
using speedCode.WsRoom.DTO;
using Xunit;
using System.Net.WebSockets;
using Moq;
using speedCode.Core.interfaces;

namespace speedCode.Tests.WsRoom;

public class WSRoomsServiceTests
{
    public WSRoomsServiceTests()
    {
        // Clear rooms before each test
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void CreateRoom_ShouldCreateNewRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();

        // Act
        var room = WSRoomsService.CreateRoom(roomId, creatorId);

        // Assert
        Assert.NotNull(room);
        Assert.Equal(roomId, room.RoomId);
        Assert.Equal(creatorId, room.CreatedBy);
        Assert.False(room.IsStarted);
        Assert.Empty(room.Players);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void CreateRoom_WhenRoomAlreadyExists_ThrowsException()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        WSRoomsService.CreateRoom(roomId, creatorId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            WSRoomsService.CreateRoom(roomId, creatorId));
        
        Assert.Contains("already exists", exception.Message);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void GetOrCreateRoom_WhenRoomExists_ReturnsExistingRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var existingRoom = WSRoomsService.CreateRoom(roomId, creatorId);

        // Act
        var room = WSRoomsService.GetOrCreateRoom(roomId, Guid.NewGuid());

        // Assert
        Assert.Same(existingRoom, room);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void GetOrCreateRoom_WhenRoomDoesNotExist_CreatesNewRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();

        // Act
        var room = WSRoomsService.GetOrCreateRoom(roomId, creatorId);

        // Assert
        Assert.NotNull(room);
        Assert.Equal(roomId, room.RoomId);
        Assert.Equal(creatorId, room.CreatedBy);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void GetRoom_WhenRoomExists_ReturnsRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var createdRoom = WSRoomsService.CreateRoom(roomId, creatorId);

        // Act
        var room = WSRoomsService.GetRoom(roomId);

        // Assert
        Assert.NotNull(room);
        Assert.Same(createdRoom, room);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void GetRoom_WhenRoomDoesNotExist_ThrowsException()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            WSRoomsService.GetRoom(roomId));
        
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void StartRoom_ByCreator_ShouldStartRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        WSRoomsService.CreateRoom(roomId, creatorId);

        // Act
        var room = WSRoomsService.StartRoom(roomId, creatorId);

        // Assert
        Assert.True(room.IsStarted);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void StartRoom_ByNonCreator_ThrowsException()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        WSRoomsService.CreateRoom(roomId, creatorId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            WSRoomsService.StartRoom(roomId, otherUserId));
        
        Assert.Contains("Only the creator", exception.Message);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void JoinRoom_ShouldAddPlayerToRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var room = WSRoomsService.CreateRoom(roomId, creatorId);
        
        var mockSocket = new Mock<WebSocket>();
        var player = new Player
        {
            Profile = new ProfileDto { id = playerId, nickName = "testuser", avatarUrl = "https://example.com/avatar.png" },
            Socket = mockSocket.Object,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = new List<Guid>()
        };

        // Act
        WSRoomsService.JoinRoom(room, player);

        // Assert
        Assert.Single(room.Players);
        Assert.True(room.Players.ContainsKey(playerId));
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void LeaveRoom_ShouldRemovePlayerFromRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var room = WSRoomsService.CreateRoom(roomId, creatorId);
        
        var mockSocket = new Mock<WebSocket>();
        var player = new Player
        {
            Profile = new ProfileDto { id = playerId, nickName = "testuser", avatarUrl = "https://example.com/avatar.png" },
            Socket = mockSocket.Object,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = new List<Guid>()
        };
        
        WSRoomsService.JoinRoom(room, player);

        // Act
        WSRoomsService.LeaveRoom(room, playerId);

        // Assert
        Assert.Empty(room.Players);
        Assert.False(WSRoomsService.Rooms.ContainsKey(roomId)); // Room should be removed when empty
    }

    [Fact]
    public void LeaveRoom_WhenLastPlayerLeaves_ShouldRemoveRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var room = WSRoomsService.CreateRoom(roomId, creatorId);
        
        var mockSocket = new Mock<WebSocket>();
        var player = new Player
        {
            Profile = new ProfileDto { id = creatorId, nickName = "creator", avatarUrl = "https://example.com/avatar.png" },
            Socket = mockSocket.Object,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = new List<Guid>()
        };
        
        WSRoomsService.JoinRoom(room, player);

        // Act
        WSRoomsService.LeaveRoom(room, creatorId);

        // Assert
        Assert.False(WSRoomsService.Rooms.ContainsKey(roomId));
    }

    [Fact]
    public void GetOnlineRoomsById_ShouldReturnRoomsCreatedByUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var room1Id = Guid.NewGuid();
        var room2Id = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        
        WSRoomsService.CreateRoom(room1Id, userId);
        WSRoomsService.CreateRoom(room2Id, userId);
        WSRoomsService.CreateRoom(Guid.NewGuid(), otherUserId);

        // Act
        var rooms = WSRoomsService.GetOnlineRoomsById(userId);

        // Assert
        Assert.Equal(2, rooms.Count);
        Assert.All(rooms, room => Assert.Equal(userId, room.CreatedBy));
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void GetOnlineRoomsById_WhenNoRooms_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var rooms = WSRoomsService.GetOnlineRoomsById(userId);

        // Assert
        Assert.Empty(rooms);
    }
}
