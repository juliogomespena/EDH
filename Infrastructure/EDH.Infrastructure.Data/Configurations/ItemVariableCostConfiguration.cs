using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

internal sealed class ItemVariableCostConfiguration : IEntityTypeConfiguration<ItemVariableCost>	
{
	public void Configure(EntityTypeBuilder<ItemVariableCost> builder)
	{
		builder.ToTable("ItemVariableCost");

		builder.HasKey(i => i.Id);

		builder.Property(i => i.Id)
			.ValueGeneratedOnAdd()
			.HasColumnName("Id");

		builder.Property(i => i.CostName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property("_valueAmount")
				.HasColumnName("Value")
				.IsRequired()
				.HasPrecision(18, 2);
		
		builder.Ignore(i => i.Value);

		builder.Property(i => i.Currency)
			.IsRequired()
			.HasConversion<string>();

		builder.HasOne(i => i.Item)
			.WithMany(ivc => ivc.ItemVariableCosts)
			.HasForeignKey(ivc => ivc.ItemId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}