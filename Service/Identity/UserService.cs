using AppUtility.Helper;
using Dapper;
using Data;
using Entities.Enums;
using Entities.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Service.Identity
{
    public class UserService : IUserService
    {
        private readonly IDapperRepository _dapper;
        private readonly ILogger<UserService> _logger;
        public UserService(IDapperRepository dapper, ILogger<UserService> logger)
        {
            _dapper = dapper;
            _logger = logger;
        }
        public async Task<IResponse> AddAsync(ApplicationUser entity)
        {
            var res = new Response();
            try
            {
                int i = await _dapper.ExecuteAsync("UpdateUser", new
                {
                    entity.Id,
                    entity.Name,
                    entity.Email,
                    entity.Gender,
                    entity.PasswordHash,
                    entity.TwoFactorEnabled,
                    entity.RefreshToken,
                    entity.RefreshTokenExpiryTime
                }, commandType: CommandType.StoredProcedure);
                var description = Utility.O.GetErrorDescription(i);
                if (i > 0 && i < 10)
                {
                    res.StatusCode = ResponseStatus.Success;
                    res.ResponseText = ResponseStatus.Success.ToString();
                }
                else
                {
                    res.ResponseText = description;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return res;
        }
        public async Task<IResponse> ChangeAction(int id)
        {
            string sqlQuery = @"UPDATE Users SET IsActive = 1^IsActive Where id = @id";
            int i = await _dapper.ExecuteAsync(sqlQuery, new { id }, CommandType.Text);
            var response = new Response();
            if (i > -1)
            {
                response.StatusCode = ResponseStatus.Success;
                response.ResponseText = ResponseStatus.Success.ToString();
            }
            return response;
        }
        public async Task<IResponse> TwoFactorEnabled(int id)
        {
            string sqlQuery = @"UPDATE Users SET TwoFactorEnabled = 1^TwoFactorEnabled Where Id = @id";
            int i = await _dapper.ExecuteAsync(sqlQuery, new { id }, CommandType.Text);
            var response = new Response();
            if (i > -1)
            {
                response.StatusCode = ResponseStatus.Success;
                response.ResponseText = ResponseStatus.Success.ToString();
            }
            return response;
        }
        public Task<IResponse> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<IResponse<ApplicationUser>> GetByIdAsync(int id)
        {
            Response<ApplicationUser> res = new Response<ApplicationUser>();
            try
            {
                var result = await _dapper.GetAsync<ApplicationUser>("proc_users", new { UserID = id }, commandType: CommandType.StoredProcedure);
                res = new Response<ApplicationUser>
                {
                    StatusCode = ResponseStatus.Success,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                res = new Response<ApplicationUser>
                {
                    StatusCode = ResponseStatus.Failed,
                    ResponseText = ResponseStatus.Failed.ToString(),
                    Result = new ApplicationUser(),
                    //Exception = ex
                };
            }
            return res;
        }
        public async Task<UserDetail> GetUserDetailAsync(int id)
        {
            string sqlQuery = @"SELECT  u.Id,Email,u.[Name],u.PhoneNumber,ur.RoleId,r.Name [Role] 
                                FROM    Users u(nolock) 
                                        INNER JOIN UserRoles ur(nolock) on u.Id = ur.UserId 
                                        INNER JOIN ApplicationRole r(nolock) on r.Id = ur.RoleId Where u.Id = @id";
            UserDetail res = await _dapper.GetAsync<UserDetail>(sqlQuery, new { id }, commandType: CommandType.Text);
            return res ?? new UserDetail();
        }
        public Task<IReadOnlyList<ApplicationUser>> GetDropdownAsync(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(int loginId = 0, ApplicationUser entity = null)
        {
            string sqlQuery = @"select u.Id ,u.UserId,u.Email,u.PhoneNumber,u.UserName,u.TwoFactorEnabled,u.[Name],u.IsActive,
                                stuff((        
                                 select ',' + cast(b.BuisnessType as varchar)      
                                 from UserBuisnesses b        
                                 where b.UserId = u.Id       
                                 for xml path('')        
                                ),1,1,'') BuisnessTypes
                                from Users(nolock) u order by Id desc";
            var res = await _dapper.GetAllAsync<ApplicationUser>(sqlQuery, entity, CommandType.Text);
            return res ?? new List<ApplicationUser>();
        }
        public async Task<IResponse> SubscribePackage(int userId, int packageId, string startFrom)
        {
            if (string.IsNullOrEmpty(startFrom))
            {
                startFrom = DateTime.Now.ToString("dd MMM yyyy");
            }
            var res = await _dapper.GetAsync<Response>("proc_SubscribePackage", new
            {
                userId,
                packageId,
                startFrom
            }, commandType: CommandType.StoredProcedure);
            return res;
        }
        public async Task<Response> Assignpackage(int TID)
        {
            var response = await _dapper.GetAsync<Response>("proc_UpdatePayment", new { TID }, commandType: CommandType.StoredProcedure);
            return response;
        }
        public async Task<DashboardSummary> GetDashboardSummaryAsync(int loginId)
        {
            var res = await _dapper.GetAsync<DashboardSummary>("proc_DashboardSummary", new { loginId }, CommandType.StoredProcedure);
            return res ?? new DashboardSummary();
        }
        public async Task<IEnumerable<string>> GetUserPackages(int userId)
        {
            string sqlQuery = @"SELECT mp.PackageName 
                                FROM UsersPackage up(nolock) inner join MASTER_PACKAGE mp(nolock) on mp.Id = up.PackageId 
                                WHERE up.UserId = @userId and up.IsActive = 1 and CAST(up.ExpiredOn as date) > CAST(GETDATE() as date)";
            var response = await _dapper.GetAllAsync<string>(sqlQuery, new { userId }, commandType: CommandType.Text);
            return response;
        }
    }
}
