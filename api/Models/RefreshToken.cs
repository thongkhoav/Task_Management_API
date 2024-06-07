using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string JwtTokenId { get; set; }
        public string Refresh_Token { get; set; }
        //We will make sure the refresh token is only valid for one use
        public bool IsValid { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}