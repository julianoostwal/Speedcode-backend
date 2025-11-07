using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Profile.model
{
    [Table("profile_achivements")]
    public class ProfileAchivement
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();

        [Required] public Guid achievementId { get; set; }

        [Required] public Guid profileId { get; set; }

        [Required] public DateTime createdAt { get; set; }
    }
}