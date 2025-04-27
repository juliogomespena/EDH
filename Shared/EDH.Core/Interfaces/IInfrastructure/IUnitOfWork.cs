namespace EDH.Core.Interfaces.IInfrastructure;

public interface IUnitOfWork : IDisposable
{
	Task<int> SaveChangesAsync();
}