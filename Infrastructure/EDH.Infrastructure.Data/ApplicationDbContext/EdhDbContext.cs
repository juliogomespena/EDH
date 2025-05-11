using System.Reflection;
using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EDH.Infrastructure.Data.ApplicationDbContext;

public sealed class EdhDbContext(DbContextOptions<EdhDbContext> options) : DbContext(options)
{
	public DbSet<Item> Items => Set<Item>();
	public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
	public DbSet<ItemVariableCost> ItemVariableCosts => Set<ItemVariableCost>();
	public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
	public DbSet<SaleLine> SaleLines => Set<SaleLine>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}