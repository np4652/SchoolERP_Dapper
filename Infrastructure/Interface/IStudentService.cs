using Data;
using Entities.Models;

namespace Infrastructure.Interface
{
    public interface IStudentService : IRepository<Student, StudentColumn>
    {
        Task<PagedResult<FeeHistory>> FeeHistory(PagedRequest request);
    }
}
