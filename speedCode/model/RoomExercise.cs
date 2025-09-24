using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("room_exercises")]
    public class RoomExercise
    {
        [Key]
        [Column(TypeName = "uuid")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public virtual string RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Rooms Room { get; set; }

        [Required]
        public virtual string ExerciseId { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise Exercise { get; set; }
    }
}