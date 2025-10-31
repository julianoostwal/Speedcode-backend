using speedCode.Room.service;
using speedCode.rooms.service;
using speedCode.WsRoom.handlers;
using speedCode.WsRoom.interfaces;

namespace speedCode.WsRoom;

public class WebSocketActionFactory
{
    private readonly JoinHandler _joinHandler;
    private readonly StartRoomHandler _startRoomHandler;
    private readonly CreateHandler _createHandler;

    public WebSocketActionFactory(JoinHandler joinHandler, CreateHandler createHandler,
        StartRoomHandler startRoomHandler)
    {
        _createHandler = createHandler;
        _joinHandler = joinHandler;
        _startRoomHandler = startRoomHandler;
    }

    public IWebSocketActionHandler? GetHandler(string action)
    {
        switch (action.ToLower())
        {
            case "join":
                return _joinHandler;

            case "start":
                return _startRoomHandler;
            case "create":
                return _createHandler;
            default:
                return null;
        }
    }
}