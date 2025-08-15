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

        builder.Property("_totalVariableCostsAmount")
                .HasColumnName("TotalVariableCosts")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(s => s.TotalVariableCosts);
        
        builder.Property("_totalProfitAmount")
                .HasColumnName("TotalProfit")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(s => s.TotalProfit);
        
        builder.Property("_totalAdjustmentAmount")
                .HasColumnName("TotalAdjustment")
                .HasDefaultValue(0)
                .HasPrecision(18, 2);
        
        builder.Ignore(s => s.TotalAdjustment);
        
        builder.Property("_totalValueAmount")
                .HasColumnName("TotalValue")
                .IsRequired()
                .HasPrecision(18, 2);
        
        builder.Ignore(s => s.TotalValue);
        
        builder.Property(s => s.Currency)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(s => s.Date)
            .IsRequired()
            .HasDefaultValueSql("DATETIME('now')");

        builder.HasMany(s => s.SaleLines)
            .WithOne(s => s.Sale)
            .OnDelete(DeleteBehavior.Cascade);
    }
}