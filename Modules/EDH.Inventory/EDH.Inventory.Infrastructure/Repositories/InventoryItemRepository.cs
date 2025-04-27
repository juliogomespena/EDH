using EDH.Infrastructure.Data.Repository;
using EDH.Core.Entities;
using EDH.Core.Interfaces.IInventory;
using EDH.Infrastructure.Data.ApplicationDbContext;

namespace EDH.Inventory.Infrastructure.Repositories;

public sealed class InventoryItemRepository(EdhDbContext dbContext) : BaseRepository<InventoryItem>(dbContext), IInventoryItemRepository
{
	
}