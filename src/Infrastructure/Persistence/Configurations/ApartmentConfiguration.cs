using Domain.Apartments;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Address)
            .HasColumnType("varchar(500)")
            .IsRequired();

        builder.HasIndex(x => x.Address)
            .IsUnique();

        builder.Property(x => x.Area)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.RoomsCount)
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.IsSold)
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}
