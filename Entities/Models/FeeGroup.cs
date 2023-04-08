using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class FeeGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
        public int FromClass { get; set; }
        public int ToClass { get; set; }

        public ICollection<ClassMaster> ClassMasters { get; set; }
    }

    public class FeeGroupColumn : FeeGroup
    {

    }
}
