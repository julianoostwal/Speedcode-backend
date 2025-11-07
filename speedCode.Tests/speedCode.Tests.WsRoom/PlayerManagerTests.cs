using speedCode.rooms;
using speedCode.WsRoom.DTO;
using Xunit;
using System.Net.WebSockets;
using Moq;
using speedCode.Core.interfaces;

namespace speedCode.Tests.WsRoom;

public class PlayerManagerTests
{
    public PlayerManagerTests()
    {
        // Clear rooms before each test
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void AddPlayer_ShouldAddPlayerToRoom()
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
        PlayerManager.AddPlayer(room, player);

        // Assert
        Assert.Single(room.Players);
        Assert.True(room.Players.ContainsKey(playerId));
        Assert.Equal(player, room.Players[playerId]);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void RemovePlayer_ShouldRemovePlayerFromRoom()
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
        
        PlayerManager.AddPlayer(room, player);

        // Act
        PlayerManager.RemovePlayer(room, playerId);

        // Assert
        Assert.Empty(room.Players);
        
        // Room should be removed when empty
        Assert.False(WSRoomsService.Rooms.ContainsKey(roomId));
    }

    [Fact]
    public void CompletedAExercise_ShouldAddExerciseToPlayerFinishedList()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var room = WSRoomsService.CreateRoom(roomId, creatorId);
        
        var mockSocket = new Mock<WebSocket>();
        mockSocket.Setup(s => s.State).Returns(WebSocketState.Open);
        
        var player = new Player
        {
            Profile = new ProfileDto { id = playerId, nickName = "testuser", avatarUrl = "https://example.com/avatar.png" },
            Socket = mockSocket.Object,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = new List<Guid>()
        };
        
        PlayerManager.AddPlayer(room, player);

        var dto = new FinishARoomExerciseDto
        {
            ExerciseId = exerciseId,
            Room = room,
            memory = 1024.5,
            time = TimeSpan.FromMilliseconds(150),
            clientId = playerId
        };

        // Act
        PlayerManager.CompletedAExercise(dto);

        // Assert
        Assert.Single(room.Players[playerId].Finishedexercise);
        Assert.Contains(exerciseId, room.Players[playerId].Finishedexercise);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void CompletedAExercise_ShouldAllowMultipleExercises()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var exerciseId1 = Guid.NewGuid();
        var exerciseId2 = Guid.NewGuid();
        var room = WSRoomsService.CreateRoom(roomId, creatorId);
        
        var mockSocket = new Mock<WebSocket>();
        mockSocket.Setup(s => s.State).Returns(WebSocketState.Open);
        
        var player = new Player
        {
            Profile = new ProfileDto { id = playerId, nickName = "testuser", avatarUrl = "https://example.com/avatar.png" },
            Socket = mockSocket.Object,
            ConnectedAt = DateTime.UtcNow,
            Finishedexercise = new List<Guid>()
        };
        
        PlayerManager.AddPlayer(room, player);

        var dto1 = new FinishARoomExerciseDto
        {
            ExerciseId = exerciseId1,
            Room = room,
            memory = 1024.5,
            time = TimeSpan.FromMilliseconds(150),
            clientId = playerId
        };

        var dto2 = new FinishARoomExerciseDto
        {
            ExerciseId = exerciseId2,
            Room = room,
            memory = 2048.0,
            time = TimeSpan.FromMilliseconds(200),
            clientId = playerId
        };

        // Act
        PlayerManager.CompletedAExercise(dto1);
        PlayerManager.CompletedAExercise(dto2);

        // Assert
        Assert.Equal(2, room.Players[playerId].Finishedexercise.Count);
        Assert.Contains(exerciseId1, room.Players[playerId].Finishedexercise);
        Assert.Contains(exerciseId2, room.Players[playerId].Finishedexercise);
        
        // Cleanup
        WSRoomsService.Rooms.Clear();
    }

    [Fact]
    public void RemovePlayer_WhenLastPlayerLeaves_ShouldRemoveRoom()
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
        
        PlayerManager.AddPlayer(room, player);
        Assert.True(WSRoomsService.Rooms.ContainsKey(roomId));

        // Act
        PlayerManager.RemovePlayer(room, creatorId);

        // Assert
        Assert.False(WSRoomsService.Rooms.ContainsKey(roomId));
    }
}
