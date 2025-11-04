using System.Net.WebSockets;
using System.Text;
using speedCode.Core.interfaces;
using speedCode.Core.judge0.dto;
using speedCode.Excise;
using speedCode.Excise.model;
using speedCode.Profile;
using speedCode.Room.service;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.services;
using speedCode.WsRoom.DTO;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class RoomWebSocketHandler
{
    private readonly RoomValidator _validator;
    private readonly RoomUserService _roomUserService;
    private readonly RoomService _roomService;
    private readonly ProfileDb _profileDb;
    private readonly ExciseDb _dbContext;
    private readonly ExciseService _exciseService;
    private readonly WSService _wsService;
    private readonly WebSocketActionFactory _webSocketActionFactory;

    public RoomWebSocketHandler(RoomValidator validator, RoomUserService roomUserService, ExciseService exciseService,
        ProfileDb profileDb,
        RoomService roomService,
        WebSocketActionFactory webSocketActionFactory,
        WSService wsService)
    {
        _webSocketActionFactory = webSocketActionFactory;
        _wsService = wsService;
        _profileDb = profileDb;
        _roomService = roomService;
        _exciseService = exciseService;
        _validator = validator;
        _roomUserService = roomUserService;
    }

    public async Task HandleAsync(Guid roomId, WebSocket socket)
    {
        var json = await _wsService.ReceiveJsonAsync(socket);
        Guid clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, json);

        if (!await _validator.RoomExistsAsync(roomId) || !await _validator.ClientExistsAsync(clientId) ||
            !await _validator.ClientAllowToJoin(roomId, clientId))
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Room or client not found",
                CancellationToken.None);
            return;
        }

        string handlerType = _wsService.ValidateAndReturnJsonProperty("type", json);
        if (await _validator.IsRoomStarted(roomId) && handlerType != "create")
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "room is started no body can join",
                CancellationToken.None);
            return;
        }

        IWebSocketActionHandler handler =
            _webSocketActionFactory.GetHandler(_wsService.ValidateAndReturnJsonProperty("type", json));

        if (handler == null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "unknown type",
                CancellationToken.None);
            return;
        }

        // hier bepaald of er een join of create of wat anders is.
        await handler.HandleAsync(socket, json);
    }

    public async Task<TestCodeAsyncResults> TestCodeAsyncForWSRoom(TestCodeRequest request, Guid roomId,
        Guid clientId)
    {
        TestCodeAsyncResults result = await _exciseService.TestCodeAsync(request);

        if (result.IsPassed)
        {
            Double averageMemory = result.TestProcess
                .Where(x => x.Logs != null && x.Logs.Any())
                .SelectMany(x => x.Logs)
                .Select(log => log.result.Memory)
                .ToList().Average();


            Double averageTime = result.TestProcess
                .Where(x => x.Logs != null && x.Logs.Any())
                .SelectMany(x => x.Logs)
                .Select(log => log.result.Time)
                .Select(t => double.Parse(t))
                .Average();


            CompletedProfileExercise completedProfileExercise = new CompletedProfileExercise()
            {
                profileId = clientId,
                exerciseId = request.ExerciseId,
                memory = averageMemory,
                time = TimeSpan.FromMilliseconds(averageTime),
            };
            _profileDb.CompletedProfileExercise.Add(completedProfileExercise);

            if (WSRoomsService.Rooms.TryGetValue(roomId, out var roomValue))
            {
                PlayerManager.CompletedAExercise(new FinishARoomExerciseDto()
                {
                    clientId = clientId,
                    ExerciseId = request.ExerciseId,
                    Room = roomValue,
                    memory = averageMemory,
                    time = TimeSpan.FromMilliseconds(averageTime),
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        return result;
    }
}