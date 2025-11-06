using System.Net.WebSockets;
using speedCode.Core.judge0.dto;
using speedCode.Exercise;
using speedCode.Exercise.model;
using speedCode.Exercise.service;
using speedCode.Profile;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.WsRoom.DTO;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class RoomWebSocketHandler
{
    private readonly RoomValidator _validator;
    private readonly ProfileDb _profileDb;
    private readonly ExerciseService _exerciseService;
    private readonly WSService _wsService;
    private readonly WebSocketActionFactory _webSocketActionFactory;

    public RoomWebSocketHandler(RoomValidator validator, ExerciseService exerciseService,
        ProfileDb profileDb,
        WebSocketActionFactory webSocketActionFactory,
        WSService wsService)
    {
        _webSocketActionFactory = webSocketActionFactory;
        _wsService = wsService;
        _profileDb = profileDb;
        _exerciseService = exerciseService;
        _validator = validator;
    }

    public async Task HandleAsync(WebSocket socket)
    {
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var json = await _wsService.ReceiveJsonAsync(socket);
                Guid clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, json);
                Guid roomId = Guid.Parse(_wsService.ValidateAndReturnJsonProperty("roomId", json));
                if (!await _validator.RoomExistsAsync(roomId) || !await _validator.ClientExistsAsync(clientId) ||
                    !await _validator.ClientAllowToJoin(roomId, clientId))
                {
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Room or client not found",
                        CancellationToken.None);
                    return;
                }

                string handlerType = _wsService.ValidateAndReturnJsonProperty("type", json);

                if (handlerType != "create" && await _validator.IsRoomStarted(roomId))
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

                await handler.HandleAsync(socket, json, roomId);
            }
        }
        catch (Exception e)
        {
            await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, e.Message,
                CancellationToken.None);
            // PlayerManager.RemovePlayer(WSRoomsService.GetRoom(roomId), clientId);
        }
    }

    public async Task<TestCodeAsyncResults> TestCodeAsyncForWSRoom(TestCodeRequest request, Guid roomId,
        Guid clientId)
    {
        TestCodeAsyncResults result = await _exerciseService.TestCodeAsync(request);

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

            await _profileDb.SaveChangesAsync();
        }

        return result;
    }
}