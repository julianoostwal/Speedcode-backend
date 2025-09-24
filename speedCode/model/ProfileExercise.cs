using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("profile_exercises")]
public class ProfileExercise
{
    [Key]
    [Column(TypeName = "uuid")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public virtual string ExerciseId { get; set; }
    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; }

    [Required]
    public virtual string Profileid { get; set; }
    [ForeignKey("Profileid")]
    public virtual Profiles Profile { get; set; }
}