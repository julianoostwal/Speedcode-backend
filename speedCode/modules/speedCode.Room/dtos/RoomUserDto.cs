namespace speedCode.Room.dtos;

public class RoomUserDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid ProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
}

