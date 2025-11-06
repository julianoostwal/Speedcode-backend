using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Exercise.model;
using speedCode.Profile.Model;

namespace speedCode.Profile.model;

[Table("created_profile_exercises")]
public class CreatedProfileExercises
{
    [Key] public Guid id { get; set; } = Guid.NewGuid();

    [Required] public virtual Guid exerciseId { get; set; }
    [ForeignKey("exerciseId")] public virtual Exercise.model.Exercise exercise { get; set; }

    [Required] public virtual Guid profileId { get; set; }
    [ForeignKey("profileId")] public virtual Profiles profile { get; set; }
}