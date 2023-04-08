using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string MotherName { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string AlternateContact { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        [ForeignKey("IdentityType")]
        public string IdentityType { get; set; }
        public string IdentityNumber { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        public DateTime DOJ { get; set; }
        public DateTime DOL { get; set; }
        public bool IsDiscontinued { get; set; }
        public int ClassId { get; set; }
        public string Section { get; set; }
    }

    public class StudentColumn : Student
    {

    }
}
