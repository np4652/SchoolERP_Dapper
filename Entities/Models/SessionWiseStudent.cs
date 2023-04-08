
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class SessionWiseStudent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionId { get; set; }        
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public char Section { get; set; }
        public bool IsNew { get; set; }
        public DateTime EntryOn { get; set; }
        public DateTime ModifyOn { get; set; }

        public Student Student { get; set; }
    }
}
