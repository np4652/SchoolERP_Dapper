using Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class FeeCollection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SessionId { get; set; }
        public decimal Amount { get; set; }
        [StringLength(140)]
        public string Variables { get; set; }
        public Month Month { get; set; }
        public PaymentMode PaymentMode { get; set; }
        [StringLength(20)]
        public string TxnNumber { get; set; }
        public DateTime EntryOn { get; set; }
    }
}
