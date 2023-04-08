
namespace Entities.Models
{
    public class DashboardSummary
    {
        public int DelevedOrder { get; set; }
        public int AllOrder { get; set; }
        public int PendingOrder { get; set; }
        public int CancelledOrder { get; set; }
    }
}
