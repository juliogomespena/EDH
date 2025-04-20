using System.Linq.Expressions;

namespace EDH.Core.Interfaces;

public interface IRepository<T> where T : class
{
	Task<T?>? GetByIdAsync(int id);
	Task<IEnumerable<T>> GetAllAsync();
	Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
	Task AddAsync(T entity);
	void UpdateAsync(T entity);
	void DeleteAsync(T entity);
}