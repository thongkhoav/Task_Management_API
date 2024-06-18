using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class CreateTaskDTO
    {
        [Required]
        [MaxLength(30, ErrorMessage = "Room name is too long.")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(100, ErrorMessage = "Room description is too long.")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        [Required]
        public Guid RoomId { get; set; }
        public Guid? UserId { get; set; }
    }
}