using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FeeHistory
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string DOB { get; set; }
        public int SessionId { get; set; }
        public decimal Amount { get; set; }
        public decimal Veriables { get; set; }
        public Month Month { get; set; }
        public PaymentModes PaymentMode{ get; set; }
        public string TxnNumber{ get; set; }
        public string EntryOn{ get; set; }
    }

    public class FeeHistoryFilter
    {
        public int SessionId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public Month Month { get; set; }
    }
}
