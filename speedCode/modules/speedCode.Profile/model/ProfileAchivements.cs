using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using speedCode.Profile.Model;

namespace speedCode.Profile.model
{
    [Table("profile_achivements")]
    public class ProfileAchivement
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Required] public virtual Guid achievementId { get; set; }
        [ForeignKey("achievementId")] public virtual Achievement achievement { get; set; }

        [Required] public virtual Guid profileId { get; set; }
        [ForeignKey("profileId")] public virtual Profiles profile { get; set; }

        [Required] public DateTime createdAt { get; set; }
    }
}