using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model;

[Table("profile_exercises")]
public class ProfileExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public virtual int ExerciseId { get; set; }
    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; }

    [Required]
    public virtual int Profileid { get; set; }
    [ForeignKey("Profileid")]
    public virtual Profiles Profile { get; set; }
}