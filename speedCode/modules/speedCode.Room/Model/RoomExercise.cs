using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Room.Model;

namespace speedCode.Exercise.model
{
    [Table("room_exercises")]
    public class RoomExercise
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Guid roomId { get; set; }
        [ForeignKey("roomId")]
        public virtual Rooms room { get; set; }

        [Required]
        public virtual Guid exerciseId { get; set; }
        [ForeignKey("exerciseId")]
        public virtual Exercise exercise { get; set; }
    }
}