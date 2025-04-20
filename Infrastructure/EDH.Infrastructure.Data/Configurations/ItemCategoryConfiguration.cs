using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

public sealed class ItemCategoryConfiguration : IEntityTypeConfiguration<ItemCategory>
{
	public void Configure(EntityTypeBuilder<ItemCategory> builder)
	{
		builder.ToTable("ItemCategory");

		builder.HasKey(i => i.Id);

		builder.Property(i => i.Id)
			.ValueGeneratedOnAdd()
			.HasColumnName("Id");

		builder.Property(i => i.Name)
			.IsRequired()
			.HasMaxLength(100);
		
		builder.Property(i => i.Description)
			.HasMaxLength(500);

		builder.HasMany(i => i.Items)
			.WithOne(i => i.ItemCategory)
			.HasForeignKey(i => i.ItemCategoryId)
			.OnDelete(DeleteBehavior.SetNull);
	}
}