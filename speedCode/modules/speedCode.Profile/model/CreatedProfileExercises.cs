using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Profile.model;

[Table("created_profile_exercises")]
public class CreatedProfileExercises
{
    [Key] public Guid id { get; set; } = Guid.NewGuid();

    [Required] public Guid exerciseId { get; set; }

    [Required] public Guid profileId { get; set; }
}