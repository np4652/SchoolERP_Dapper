using Data;
using Entities.Models;
using System.Linq.Expressions;

namespace Infrastructure.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<IResponse<T>> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(int loginId = 0, T entity = null);
        Task<IResponse> AddAsync(T entity);
        Task<IResponse> DeleteAsync(int id);
        Task<IReadOnlyList<T>> GetDropdownAsync(T entity);
    }

    public interface IRepository<TRow, TColumn> where TRow : class where TColumn : class
    {
        Task<TColumn> GetByIdAsync(int id);
        Task<PagedResult<TColumn>> GetAsync(PagedRequest request);
        Task<IResponse> AddAsync(TRow entity);
        Task<IResponse> DeleteAsync(int id);
    }
}
