using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Voyagr.Domain.Entities;

namespace Voyagr.Infrastructure.Data.Configurations
{
    public class TripImageConfiguration
    : IEntityTypeConfiguration<TripImage>
    {
        public void Configure(
            EntityTypeBuilder<TripImage> builder)
        {
            builder.ToTable("TripImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.PublicId)
                .HasMaxLength(500);

            builder.Property(x => x.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.Trip)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TripId);
        }
    }
}
