using System.ComponentModel.DataAnnotations;

namespace Entities.Enums
{
    public enum UserRoles
    {
        Admin = 1,
        User = 2,
        DeliveryPerson = 3,
        Kisaan = 4,
        [Display(Name = "Farmers Producers Organization")]
        FPO = 5
    }
}
