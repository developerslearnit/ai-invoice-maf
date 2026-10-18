using InvoiceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceApp.Infrastructure.Persistence.Configurations;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.InvoiceId)
            .IsRequired();

        builder.Property(item => item.Description)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Ignore(item => item.LineTotal);
    }
}
