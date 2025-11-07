using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Profile.model
{
    [Table("completed_profile_exercises")]
    public class CompletedProfileExercise
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid exerciseId { get; set; }

        [Required]
        public Guid profileId { get; set; }
        
        [Required]
        public Double memory { get; set; }
        [Required]
        public TimeSpan time{get;set;}
    }
}