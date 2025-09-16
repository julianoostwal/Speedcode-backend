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
        [ForeignKey("Room")]
        public Rooms RoomId { get; set; }

        [Required]
        [ForeignKey("Exercise")]
        public Exercise ExerciseId { get; set; }
    }
}