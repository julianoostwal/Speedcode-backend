using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("profile_exercises")]
    public class ProfileExercise
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        public virtual Guid exerciseId { get; set; }
        [ForeignKey("exerciseId")]
        public virtual Exercise exercise { get; set; }

        [Required]
        public virtual Guid profileId { get; set; }
        [ForeignKey("profileId")]
        public virtual Profiles profile { get; set; }
    }
}