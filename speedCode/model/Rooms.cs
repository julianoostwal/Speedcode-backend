using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace speedCode.Model
{
    [Table("rooms")]
    public class Rooms
    {
        [Key]
        [Column(TypeName = "uuid")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column(TypeName = "text")]
        public string RoomName { get; set; }


        public enum Level { Easy, Medium, Hard }
        public Level RoomLevel { get; set; }

        [Required]
        [Column(TypeName = "boolean")]
        public bool IsPrivate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}