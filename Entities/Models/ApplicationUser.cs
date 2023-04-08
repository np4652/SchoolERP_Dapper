using Entities.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class ApplicationUser : ApplicationUserProcModel
    {
        public decimal Balance { get; set; }
        public UserRoles RoleId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; }
        public string BuisnessTypes { get; set; }
    }


    public class ApplicationUserProcModel : IdentityUser<int>
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        [StringLength(1)]
        public string Gender { get; set; }
        public string GAuthPin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string Token { get; set; }
    }

    public class UserUpdateRequest
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string BussinessTypes { get; set; }


        public AuthenticateResponse(ApplicationUser user, string token)
        {
            user = user ?? new ApplicationUser();
            Id = user.Id;
            Username = user.UserName;
            Role = user.Role;
            Name = user.Name;
            RefreshToken = user.RefreshToken;
            Token = token;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            Gender = user.Gender;
            BussinessTypes = user.BuisnessTypes;
        }
    }

    public class ApplicationUserResponse
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public UserRoles RoleId { get; set; }
        public string Role { get; set; }   
        public string name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool IsActive { get; set; }
        public string BussinessType { get; set; }
    }
}
