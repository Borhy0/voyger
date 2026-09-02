using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyagr.Domain.Entities;

namespace Voyagr.Infrastructure.Data.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trips");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Destination)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Country)
                .HasMaxLength(100);

            builder.Property(x => x.Latitude);

            builder.Property(x => x.Longitude);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Travelers)
                .IsRequired();

            builder.Property(x => x.BudgetTotal)
                .HasPrecision(18, 2);

            builder.Property(x => x.IsSavedOffline)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Trip belongs to one User
            builder.HasOne(x => x.User)
                .WithMany(x => x.Trips)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
