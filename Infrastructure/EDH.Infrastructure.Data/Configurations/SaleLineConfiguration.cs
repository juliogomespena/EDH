using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

internal sealed class SaleLineConfiguration :IEntityTypeConfiguration<SaleLine>
{
    public void Configure(EntityTypeBuilder<SaleLine> builder)
    {
        builder.ToTable("SaleLine");
        
        builder.HasKey(sl => sl.Id);

        builder.Property(sl => sl.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        builder.Property("_unitPriceAmount")
                .HasColumnName("UnitPrice")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(sl => sl.UnitPrice);

        builder.OwnsOne(sl => sl.Quantity, quantity =>
        {
            quantity.Property(v => v.Value)
                .HasColumnName("Quantity")
                .IsRequired();
        });
        
        builder.Property("_unitVariableCostsAmount")
                .HasColumnName("UnitVariableCosts")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(sl => sl.UnitVariableCosts);
        
        builder.Property("_totalVariableCostsAmount")
                .HasColumnName("TotalVariableCosts")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(sl => sl.TotalVariableCosts);
        
        builder.Property("_adjustmentAmount")
                .HasColumnName("Adjustment")
                .HasDefaultValue(0)
                .HasPrecision(18, 2);
        
        builder.Ignore(sl => sl.Adjustment);
        
        builder.Property("_profitAmount")
                .HasColumnName("Profit")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(sl => sl.Profit);
        
        builder.Property("_subtotalAmount")
                .HasColumnName("Subtotal")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(sl => sl.Subtotal);
        
        builder.Property(sl => sl.Currency)
            .IsRequired()
            .HasConversion<string>();
        
        builder.HasOne(sl => sl.Item)
            .WithMany(i => i.SaleLines)
            .HasForeignKey(sl => sl.ItemId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(s => s.Sale)
            .WithMany(s => s.SaleLines)
            .HasForeignKey(sl => sl.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}