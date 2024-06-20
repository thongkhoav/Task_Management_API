using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class RemoveMemberDTO
    {
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
    }
}