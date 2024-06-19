using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class UpdateTaskDTO
    {
        public Guid Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsComplete { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? UserId { get; set; }
    }
}