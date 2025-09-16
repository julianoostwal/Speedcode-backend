using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("profile_exercises")]
public class ProfileExercise
{
    [Key] public Guid Id { get; set; }

    [Required] public Exercise Exercise { get; set; }

    [Required] public Profiles Profile { get; set; }
}