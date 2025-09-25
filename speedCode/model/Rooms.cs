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
        public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        [Column(TypeName = "text")]
        public string roomName { get; set; }


        public enum Level { Easy, Medium, Hard }
        public Level roomLevel { get; set; }

        [Required]
        [Column(TypeName = "boolean")]
        public bool isPrivate { get; set; }

        [Required]
        public DateTime createdAt { get; set; }
    }
}