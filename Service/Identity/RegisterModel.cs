using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Service.Identity
{
    public class RegisterModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select role")]
        public UserRoles UserRole { get; set; } = UserRoles.User;

        [Required(ErrorMessage = "Please enter name")]
        [StringLength(200)]
        [RegularExpression(@"[a-zA-Z ]*$", ErrorMessage = "Only Alphabets are Allowed")]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(0|91)?[6-9][0-9]{9}", ErrorMessage = "Not a valid phone number")]
        [Required(ErrorMessage = "Please Enter Mobile No")]
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int AreaId { get; set; }

        [RegularExpression(@"\d+", ErrorMessage = "Only numbers are Allowed")]
        public string? OTP { get; set; } = "0";
        
        [Required,StringLength(1),RegularExpression(@"^(M|F|O)")]
        public string Gender { get; set; }
    }
}
