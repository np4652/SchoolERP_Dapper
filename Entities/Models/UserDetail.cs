
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class UserDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }        
        public string Gender { get; set; }
    }

    public class UserDetailRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [StringLength(1)]
        public string Gender { get; set; }
    }
}
