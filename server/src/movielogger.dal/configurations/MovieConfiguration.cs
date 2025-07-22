using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using movielogger.dal.entities;

namespace movielogger.dal.configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Title).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Description).IsRequired();
        builder.Property(m => m.ReleaseDate).IsRequired();
        builder.Property(m => m.PosterPath).HasMaxLength(500).IsRequired(false);
        builder.HasOne(m => m.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(m => m.GenreId);
    }
}