using EDH.Core.Entities;
using EDH.Core.Interfaces.IItems;
using EDH.Infrastructure.Data.ApplicationDbContext;
using EDH.Infrastructure.Data.Repository;

namespace EDH.Items.Infrastructure.Repositories;

public sealed class ItemRepository(EdhDbContext dbContext) : BaseRepository<Item>(dbContext), IItemRepository
{
	
}