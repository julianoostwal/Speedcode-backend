using Microsoft.EntityFrameworkCore;
using speedCode.Room.Model;

namespace speedCode.Room;

public class RoomsDb : DbContext
{
    public DbSet<Rooms> rooms { get; set; }
}