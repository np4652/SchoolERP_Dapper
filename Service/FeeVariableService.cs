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
    public class FeeVariableService : IFeeVaribaleService
    {
        private readonly IDapperRepository _repository;
        public FeeVariableService(IDapperRepository repository)
        {

            _repository=repository;

        }

        public async Task<IResponse> AddAsync(FeeVaribale entity)
        {
            Response res = new Response();
            try
            {
                string sqlQuery = @"INSERT INTO FeeVaribale(Name,FeeGroupId,Cost,IsAdditional,Description,EntryOn,ModifyOn) 
                                                     VALUES(@Name,@FeeGroupId,@Cost,@IsAdditional,@Description,GETDATE(),GETDATE())";
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

        public async Task<PagedResult<FeeVaribaleColumn>> GetAsync(PagedRequest request)
        {
            var res = new PagedResult<FeeVaribaleColumn>();
            try
            {
                string tableName = "FeeVaribale(nolock)";
                string sqlQuery = @$"SELECT * FROM {tableName};
                                 SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize FROM {tableName}";

                var result = await _repository.GetMultipleAsync<PagedResult<FeeVaribaleColumn>, FeeVaribaleColumn>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize
                    }, CommandType.Text);

                var data = (List<FeeVaribaleColumn>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<FeeVaribaleColumn>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public Task<FeeVaribaleColumn> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
