using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("UserRooms")]
    public class UserRoom
    {

        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public ApplicationUser? User { get; set; }
        public Room? Room { get; set; }
    }
}