using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<UserRoom> UserRooms { get; set; } = [];
        public ICollection<Task> Tasks { get; set; } = [];
    }
}