using Data;
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
    public class ClassMasterService : IClassMasterService
    {
        private readonly IDapperRepository _repository;
        public ClassMasterService(IDapperRepository repository)
        {

            _repository=repository;

        }

        public async Task<IResponse> AddAsync(ClassMaster entity)
        {
            Response res = new Response();
            try
            {
                string sqlQuery = @"INSERT INTO ClassMaster(ClassName) VALUES(@ClassName)";
                var result = await _repository.ExecuteAsync(sqlQuery, entity, commandType: CommandType.Text);

                res = new Response
                {
                    StatusCode = result != -1 ? ResponseStatus.Success : ResponseStatus.Failed,
                    ResponseText = result != -1 ? ResponseStatus.Success.ToString() : ResponseStatus.Failed.ToString(),
                };
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

        public async Task<PagedResult<ClassMasterColumn>> GetAsync(PagedRequest request)
        {
            var res = new PagedResult<ClassMasterColumn>();
            try
            {
                string tableName = "CLASSMASTER(nolock)";
                string sqlQuery = @$"SELECT * FROM {tableName};
                                 SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize FROM {tableName}";

                var result = await _repository.GetMultipleAsync<PagedResult<ClassMasterColumn>, ClassMasterColumn>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize
                    }, CommandType.Text);

                var data = (List<ClassMasterColumn>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<ClassMasterColumn>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public async Task<ClassMasterColumn> GetByIdAsync(int id)
        {
            ClassMasterColumn res = new ClassMasterColumn();
            
            try
            {
                string sqlQuery = @"";
                res = await _repository.GetAsync<ClassMasterColumn>(sqlQuery, new { id }, CommandType.Text);
            
            }
            catch (Exception ex)
            {
                
            }
            return res;
        }
    }
}
