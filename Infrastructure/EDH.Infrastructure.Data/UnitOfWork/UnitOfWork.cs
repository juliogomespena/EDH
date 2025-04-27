using EDH.Core.Interfaces.IInfrastructure;
using EDH.Infrastructure.Data.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace EDH.Infrastructure.Data.UnitOfWork;

public sealed class UnitOfWork(EdhDbContext dbContext) : IUnitOfWork
{
	private bool _disposed;
	private readonly EdhDbContext _dbContext = dbContext;

	public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			_dbContext.Dispose();
		}
		_disposed = true;
	}
}