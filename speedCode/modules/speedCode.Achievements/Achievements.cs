using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Achievements
{
    [Table("achievements")]
    public class Achievement
    {
        [Key] public Guid id { get; set; } = Guid.NewGuid();
        public string achievementIcon { get; set; }

        public string achievementName { get; set; }

        public string achievementDescription { get; set; }
    }
}