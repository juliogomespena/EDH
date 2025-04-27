using EDH.Core.Interfaces.IInfrastructure;
using EDH.Infrastructure.Data.ApplicationDbContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace EDH.Infrastructure.Data.UnitOfWork;

public sealed class UnitOfWork(EdhDbContext dbContext) : IUnitOfWork
{
	private bool _disposed;
	private readonly EdhDbContext _dbContext = dbContext;
	private IDbContextTransaction? _transaction;

	public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

	public async Task BeginTransactionAsync()
	{
		_transaction = await _dbContext.Database.BeginTransactionAsync();
	}

	public async Task CommitTransactionAsync()
	{
		if (_transaction is null)
		{
			throw new InvalidOperationException("Transaction has not been started.");
		}

		await _transaction.CommitAsync();
		await DisposeTransactionAsync();
	}

	public async Task RollbackTransactionAsync()
	{
		if (_transaction is null)
		{
			throw new InvalidOperationException("Transaction has not been started.");
		}

		await _transaction.RollbackAsync();
		await DisposeTransactionAsync();
	}

	private async Task DisposeTransactionAsync()
	{
		if (_transaction is not null)
		{
			await _transaction.DisposeAsync();
			_transaction = null;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			DisposeTransactionAsync().GetAwaiter().GetResult();
			_dbContext.Dispose();
		}
		_disposed = true;
	}
}