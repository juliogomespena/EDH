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
        
        builder.Property(sl => sl.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(sl => sl.Quantity)
            .IsRequired();
        
        builder.Property(sl => sl.UnitVariableCosts)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(sl => sl.TotalVariableCosts)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(sl => sl.Adjustment)
            .HasPrecision(18, 2);
        
        builder.Property(sl => sl.UnitVariableCosts)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(sl => sl.Profit)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(sl => sl.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);
        
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