using Data;
using Entities.Models;
using Infrastructure.Interface;

namespace Service.Identity
{
    public interface IUserService : IRepository<ApplicationUser>
    {
        Task<IResponse> ChangeAction(int id);
        Task<IResponse> TwoFactorEnabled(int id);        
        Task<DashboardSummary> GetDashboardSummaryAsync(int loginId);        
        Task<UserDetail> GetUserDetailAsync(int id);
    }
}
