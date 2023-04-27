using Data;
using Entities;
using Entities.Enums;
using Entities.Models;
using Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SessionMasterService : ISessionMasterService
    {
        private readonly IDapperRepository _repository;
        public SessionMasterService(IDapperRepository repository)
        {
            _repository = repository;
        }
        public async Task<IResponse> AddAsync(SessionMaster entity)
        {
            Response res = new Response();
            try
            {
                string sqlQuery = @"INSERT INTO SESSIONMASTER(Name,TotalStudent,NewStudent,TotalTeacher,NewTeacher,TotalEmployee,NewEmployee)
                                                       Values(@Name,@TotalStudent,@NewStudent,@TotalTeacher,@NewTeacher,@TotalEmployee,@NewEmployee)";
                var result = await _repository.ExecuteAsync(sqlQuery, entity, commandType: CommandType.Text);

                if (result > -1 && result < 50)
                {
                    res = new Response
                    {
                        StatusCode = ResponseStatus.Success,
                        ResponseText = ResponseStatus.Success.ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                res = new Response
                {
                    StatusCode = ResponseStatus.Failed,
                    ResponseText = ResponseStatus.Failed.ToString(),
                };
            }
            return res;
        }

        public Task<IResponse> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<SessionMasterColumn>> GetAsync(PagedRequest request)
        {
            //request = request ?? new PagedRequest
            //{
            //    PageNumber = 1,
            //    PageSize = 50
            //};
            var res = new PagedResult<SessionMasterColumn>();
            try
            {
                string tableName = "SESSIONMASTER(nolock)";
                string sqlQuery = @$"SELECT * FROM {tableName};
                                 SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize FROM {tableName}";

                var result = await _repository.GetMultipleAsync<SessionMasterColumn, PagedResult<SessionMasterColumn>>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize
                    }, CommandType.Text);

                var data = (List<SessionMasterColumn>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<SessionMasterColumn>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public Task<IResponse<PagedResult<IEnumerable<TColumn>>>> GetAsync<TColumn>(int loginId = 0, Expression<Func<TColumn, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task<SessionMasterColumn> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
