using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class AddMemberDTO
    {
        public string Email { get; set; } = "";
        public Guid RoomId { get; set; }
    }
}