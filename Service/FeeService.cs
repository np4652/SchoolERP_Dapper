using Data;
using Entities.Models;
using Infrastructure.Interface;
using System.Data;

namespace Service
{
    public class FeeService : IFeeService
    {
        private readonly IDapperRepository _repository;
        public FeeService(IDapperRepository repository)
        {
            _repository=repository;
        }

        public async Task<PagedResult<FeeHistory>> FeeHistory(PagedRequest request)
        {
            var filter = new FeeHistoryFilter();
            if (request.Param!=null)
            {
                filter = (FeeHistoryFilter)request.Param;
            }
            var res = new PagedResult<FeeHistory>();
            try
            {
                var result = await _repository.GetMultipleAsync<FeeHistory, PagedResult<FeeHistory>>("proc_GetFeeHistory",
                    new
                    {
                        request.PageNumber,
                        request.PageSize,
                        filter.SessionId,
                        filter.StudentId,
                        filter.Month,
                        filter.ClassId
                    }, CommandType.StoredProcedure);

                var data = (List<FeeHistory>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<FeeHistory>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public async Task<IEnumerable<FeeVaribale>> EstimatedFee(int studentId, int classId, int sessionId = 0, int month = 0)
        {
            var res = new List<FeeVaribale>();
            try
            {
                res = await _repository.GetAsync<List<FeeVaribale>>("proc_EstimatedFee",
                    new
                    {
                        studentId,
                        classId,
                        sessionId,
                        month
                    }, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
}
