using System.Linq.Expressions;
using EDH.Infrastructure.Data.Repository;
using EDH.Core.Entities;
using EDH.Core.Interfaces.IInventory;
using EDH.Infrastructure.Data.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace EDH.Inventory.Infrastructure.Repositories;

public sealed class InventoryItemRepository(EdhDbContext dbContext) : BaseRepository<InventoryItem>(dbContext), IInventoryItemRepository
{
	private readonly DbSet<InventoryItem> _dbSet = dbContext.Set<InventoryItem>();

	public override async Task<IEnumerable<InventoryItem>> FindAsync(Expression<Func<InventoryItem, bool>> predicate) =>
		await _dbSet
			.Include(inventoryItem => inventoryItem.Item)
			.Where(predicate).ToListAsync();
}