using EDH.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDH.Infrastructure.Data.Configurations;

internal sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sale");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");
        
        builder.Property(s => s.TotalVariableCosts)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalProfit)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalAdjustment)
            .HasPrecision(18, 2);
        
        builder.Property(s => s.TotalValue)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(s => s.Date)
            .IsRequired()
            .HasDefaultValueSql("DATETIME('now')");

        builder.HasMany(s => s.SaleLines)
            .WithOne(s => s.Sale)
            .OnDelete(DeleteBehavior.Cascade);
    }
}