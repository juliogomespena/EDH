using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

internal sealed class InventoryItemConfiguration:IEntityTypeConfiguration<InventoryItem>
{
	public void Configure(EntityTypeBuilder<InventoryItem> builder)
	{
		builder.ToTable("InventoryItem");

		builder.HasKey(i => i.Id);

		builder.Property(i => i.Id)
			.ValueGeneratedOnAdd()
			.HasColumnName("Id");

		builder.OwnsOne(i => i.Quantity, quantity =>
		{
			quantity.Property(v => v.Value)
				.HasColumnName("Quantity")
				.IsRequired();
		});

		builder.OwnsOne(i => i.AlertThreshold, threshold =>
		{
			threshold.Property(v => v.Value)
				.HasColumnName("AlertThreshold");
		});

		builder.Property(i => i.LastUpdated)
			.IsRequired()
			.HasDefaultValueSql("DATETIME('now')");

		builder.HasOne(i => i.Item)
			.WithOne(i => i.Inventory) 
			.HasForeignKey<InventoryItem>(i => i.ItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}