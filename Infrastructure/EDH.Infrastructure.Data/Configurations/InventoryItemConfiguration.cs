using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

public class InventoryItemConfiguration:IEntityTypeConfiguration<InventoryItem>
{
	public void Configure(EntityTypeBuilder<InventoryItem> builder)
	{
		builder.ToTable("InventoryItem");

		builder.HasKey(i => i.Id);

		builder.Property(i => i.Id)
			.ValueGeneratedOnAdd()
			.HasColumnName("Id");

		builder.Property(i => i.Quantity)
			.IsRequired()
			.HasDefaultValue(0);

		builder.Property(i => i.AlertThreshold)
			.HasDefaultValue(0);

		builder.Property(i => i.LastUpdated)
			.IsRequired()
			.HasDefaultValueSql("DATETIME('now')");

		builder.HasOne(i => i.Item)
			.WithOne(i => i.Inventory) 
			.HasForeignKey<InventoryItem>(i => i.ItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}