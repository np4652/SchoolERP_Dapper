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
    public class FeeGroupService : IFeeGroupService
    {
        private readonly IDapperRepository _repository;
        public FeeGroupService(IDapperRepository repository)
        {

            _repository=repository;

        }

        public async Task<IResponse> AddAsync(FeeGroup entity)
        {
            Response res = new Response();
            try
            {
                string sqlQuery = @"INSERT INTO FeeGroup(Description,FromClass,ToClass) VALUES(@Description,@FromClass,@ToClass)";
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

        public async Task<PagedResult<FeeGroupColumn>> GetAsync(PagedRequest request)
        {
            var res = new PagedResult<FeeGroupColumn>();
            try
            {
                string tableName = "FeeGroup(nolock)";
                string sqlQuery = @$"SELECT * FROM {tableName};
                                 SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize FROM {tableName}";

                var result = await _repository.GetMultipleAsync<FeeGroupColumn, PagedResult<FeeVaribaleColumn>>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize
                    }, CommandType.Text);

                var data = (List<FeeGroupColumn>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<FeeGroupColumn>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public Task<FeeGroupColumn> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
