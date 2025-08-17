using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

internal sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
	public void Configure(EntityTypeBuilder<Item> builder)
	{
		builder.ToTable("Item");

		builder.HasKey(i => i.Id);

		builder.Property(i => i.Id)
			.ValueGeneratedOnAdd()
			.HasColumnName("Id");

		builder.Property(i => i.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(i => i.Description)
			.HasMaxLength(500);

		builder.Property("_sellingPriceAmount")
				.HasColumnName("SellingPrice")
				.IsRequired()
				.HasPrecision(18, 2);
		
		builder.Ignore(i => i.SellingPrice);
		
		builder.Property(i => i.Currency)
			.IsRequired()
			.HasConversion<string>();
			
		builder.HasOne(i => i.ItemCategory)
			.WithMany(i => i.Items)
			.HasForeignKey(i => i.ItemCategoryId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.HasMany(i => i.ItemVariableCosts)
			.WithOne(ivc => ivc.Item)
			.HasForeignKey(ivc => ivc.ItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}