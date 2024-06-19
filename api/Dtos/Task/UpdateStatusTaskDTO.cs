using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class UpdateStatusTaskDTO
    {
        public Guid Id { get; set; }
        public bool IsComplete { get; set; }
    }
}