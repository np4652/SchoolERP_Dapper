using Data;
using Entities.Models;


namespace Infrastructure.Interface
{
    public interface IFeeService
    {
        Task<PagedResult<FeeHistory>> FeeHistory(PagedRequest request);
        Task<IEnumerable<FeeVaribale>> EstimatedFee(int studentId, int classId, int sessionId = 0, int month = 0);
    }
}
