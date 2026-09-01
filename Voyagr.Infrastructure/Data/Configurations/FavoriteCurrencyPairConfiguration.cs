using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyagr.Domain.Entities;

namespace Voyagr.Infrastructure.Data.Configurations;

public class FavoriteCurrencyPairConfiguration
    : IEntityTypeConfiguration<FavoriteCurrencyPair>
{
    public void Configure(
        EntityTypeBuilder<FavoriteCurrencyPair> builder)
    {
        builder.ToTable("FavoriteCurrencyPairs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FromCurrency)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.ToCurrency)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.FavoriteCurrencyPairs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent the same user from
        // adding the same currency pair twice.
        builder.HasIndex(
            x => new
            {
                x.UserId,
                x.FromCurrency,
                x.ToCurrency
            })
            .IsUnique();
    }
}