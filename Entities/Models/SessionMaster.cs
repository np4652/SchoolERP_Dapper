using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class SessionMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required,StringLength(9)]
        public string Name { get; set; }
        public int TotalStudent { get; set; }
        public int NewStudent { get; set; }
        public int TotalTeacher { get; set; }
        public int NewTeacher { get; set; }
        public int TotalEmployee { get; set; }
        public int NewEmployee { get; set; }
    }

    public class SessionMasterColumn: SessionMaster
    {

    }
}
