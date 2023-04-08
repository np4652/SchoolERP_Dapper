
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class FeeVaribale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(80)]
        public string Name { get; set; }

        [ForeignKey("FeeGroup")]
        public int FeeGroupId { get; set; }
        public decimal Cost { get; set; }
        public bool IsAdditional { get; set; }

        [StringLength(160)]
        public string Description { get; set; }
        
    }

    public class FeeVaribaleColumn : FeeVaribale
    {
        public DateTime EntryOn { get; set; }
        public DateTime ModifyOn { get; set; }
    }
}
