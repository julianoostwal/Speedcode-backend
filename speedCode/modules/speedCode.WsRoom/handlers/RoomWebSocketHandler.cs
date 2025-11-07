using System.Net.WebSockets;
using speedCode.Core.judge0.dto;
using speedCode.Core.interfaces;
using speedCode.rooms;
using speedCode.rooms.service;
using speedCode.WsRoom.DTO;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom.handlers;

public class RoomWebSocketHandler
{
    private readonly RoomValidator _validator;
    private readonly IExerciseTestService _exerciseTestService;
    private readonly IProfileCompletionService _profileCompletionService;
    private readonly WSService _wsService;
    private readonly WebSocketActionFactory _webSocketActionFactory;

    public RoomWebSocketHandler(
        RoomValidator validator,
        IExerciseTestService exerciseTestService,
        IProfileCompletionService profileCompletionService,
        WebSocketActionFactory webSocketActionFactory,
        WSService wsService)
    {
        _webSocketActionFactory = webSocketActionFactory;
        _wsService = wsService;
        _profileCompletionService = profileCompletionService;
        _exerciseTestService = exerciseTestService;
        _validator = validator;
    }

    public async Task HandleAsync(WebSocket socket)
    {
        Guid clientId = Guid.Empty;
        Guid roomId = Guid.Empty;
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var json = await _wsService.ReceiveJsonAsync(socket);
                clientId = await _wsService.ValidateAndExtractClientIdAsync(socket, json);
                roomId = Guid.Parse(_wsService.ValidateAndReturnJsonProperty("roomId", json));
                if (!await _validator.RoomExistsAsync(roomId) || !await _validator.ClientExistsAsync(clientId) ||
                    !await _validator.ClientAllowToJoin(roomId, clientId))
                {
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Room or client not found",
                        CancellationToken.None);
                    return;
                }

                string handlerType = _wsService.ValidateAndReturnJsonProperty("type", json);

                // Don't allow joining if room is already started
                if (handlerType == "join" && _validator.IsRoomStarted(roomId))
                {
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation,
                        "Room is already started, no one can join",
                        CancellationToken.None);
                    return;
                }

                IWebSocketActionHandler handler =
                    _webSocketActionFactory.GetHandler(handlerType);

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
            PlayerManager.RemovePlayer(WSRoomsService.GetRoom(roomId), clientId);
        }
        finally
        {
            PlayerManager.RemovePlayer(WSRoomsService.GetRoom(roomId), clientId);
            if (socket.State != WebSocketState.Closed)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        }
    }

    public async Task<TestCodeAsyncResults> TestCodeAsyncForWSRoom(TestCodeRequest request, Guid roomId,
        Guid clientId)
    {
        TestCodeAsyncResults result = await _exerciseTestService.TestCodeAsync(request);

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


            await _profileCompletionService.SaveCompletedExerciseAsync(
                clientId,
                request.ExerciseId,
                averageMemory,
                TimeSpan.FromMilliseconds(averageTime)
            );

            PlayerManager.CompletedAExercise(new FinishARoomExerciseDto()
            {
                memory = averageMemory,
                time = TimeSpan.FromMilliseconds(averageTime),
                Room = WSRoomsService.GetRoom(roomId),
                clientId = clientId,
                ExerciseId = request.ExerciseId
            });
        }

        return result;
    }
}