using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Model
{
    [Table("rooms")]
    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string RoomName { get; set; }      
    }
}