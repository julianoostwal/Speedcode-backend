using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Profile.Model;

namespace speedCode.Exercise.model
{
    [Table("completed_profile_exercises")]
    public class CompletedProfileExercise
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
        
        [Required]
        public Double memory { get; set; }
        [Required]
        public TimeSpan time{get;set;}
    }
}