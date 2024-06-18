using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("Rooms")]
    public class Room : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        // auto increment invite code
        public int InviteCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<UserRoom> UserRooms { get; set; } = [];
        public ICollection<TaskModel> Tasks { get; set; } = [];
    }
}