using EDH.Core.Entities;
using EDH.Core.Interfaces.Items;
using EDH.Infrastructure.Data.ApplicationDbContext;
using EDH.Infrastructure.Data.Repository;

namespace EDH.Items.Infrastructure.Repositories;

public class ItemRepository(EdhDbContext dbContext) : RepositoryBase<Item>(dbContext), IItemRepository
{
	private readonly EdhDbContext _dbContext = dbContext;
}