using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class CreateRoomDTO
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Room name is too long.")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(100, ErrorMessage = "Room description is too long.")]
        public string Description { get; set; } = string.Empty;
    }
}