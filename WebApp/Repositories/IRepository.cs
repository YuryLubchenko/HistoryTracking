using System.Linq.Expressions;
using WebApp.Entities;

namespace WebApp.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetById(long id);
    Task<List<T>> GetAll();
    Task<List<T>> GetAll(Expression<Func<T, bool>> filter);
    Task<long> NextIdAsync();
    Task<long> Add(T entity);
    Task<int> Update(T entity);
    Task<int> Delete(long id);
}
