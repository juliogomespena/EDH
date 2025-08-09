using EDH.Core.Entities;
using EDH.Core.Interfaces.ISales;
using EDH.Infrastructure.Data.ApplicationDbContext;
using EDH.Infrastructure.Data.Repository;

namespace EDH.Sales.Infrastructure.Repositories;

public sealed class SaleRepository(EdhDbContext dbContext) : BaseRepository<Sale>(dbContext), ISaleRepository
{
    
}