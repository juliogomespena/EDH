namespace EDH.Core.Interfaces.Infrastructure;

public interface IUnitOfWork : IDisposable
{
	Task<int> SaveChangesAsync();
}