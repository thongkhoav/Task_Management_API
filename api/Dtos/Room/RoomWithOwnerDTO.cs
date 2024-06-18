using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Room
{
    public class RoomWithOwnerDTO
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomDescription { get; set; }
        public UserDTO? Owner { get; set; }
    }
}