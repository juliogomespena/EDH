using System.Linq.Expressions;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Infrastructure.Data.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace EDH.Infrastructure.Data.Repository;

public abstract class BaseRepository<T>(EdhDbContext dbContext) : IRepository<T> where T : class
{
	private readonly DbSet<T> _dbSet = dbContext.Set<T>();

	public async Task<T?>? GetByIdAsync(int id) => await _dbSet.FindAsync(id);

	public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

	public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

	public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

	public void UpdateAsync(T entity) => _dbSet.Update(entity);

	public void DeleteAsync(T entity) => _dbSet.Remove(entity);
}