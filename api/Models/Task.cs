using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Tasks")]
    public class Task
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsComplete { get; set; }

        public Guid? UserId { get; set; }
        public Guid RoomId { get; set; }
        public ApplicationUser? User { get; set; }
        public Room? Room { get; set; }
    }
}