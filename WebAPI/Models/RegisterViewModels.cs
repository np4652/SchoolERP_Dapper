using Service.Identity;

namespace WebAPI.Models
{
    public class RegisterViewModels : RegisterModel
    {
        //[Required]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Confirm password is required")]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Confirm password did not match..")]
        //public string ConfirmPassword { get; set; }
    }
}
