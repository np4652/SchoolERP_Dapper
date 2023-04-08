using Entities.Models;

namespace Infrastructure.Interface
{
    public interface IStudentService : IRepository<Student, StudentColumn>
    {
        
    }
}
