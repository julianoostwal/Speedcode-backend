using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("room_exercises")]
    public class RoomExercise
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Rooms Room { get; set; }

        [Required]
        public Exercise Exercise { get; set; }
    }
}