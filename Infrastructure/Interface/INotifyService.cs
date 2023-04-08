using Data;
using Entities.Models;

namespace Infrastructure.Interface
{
    public interface INotifyService
    {
        Task<IResponse> SaveSMSEmailWhatsappNotification(SMSEmailWhatsappNotification req, int LoginID);
        Task<IResponse<IEnumerable<NotifyModel>>> GetNotify();
    }
}
