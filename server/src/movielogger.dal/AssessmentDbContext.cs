using Microsoft.EntityFrameworkCore;
using movielogger.dal.entities;

namespace movielogger.dal;

public class AssessmentDbContext : DbContext
{
    public AssessmentDbContext(DbContextOptions<AssessmentDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<UserMovie> UserMovies => Set<UserMovie>();
    public DbSet<Viewing> Viewings => Set<Viewing>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("master");
        
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Password).IsRequired();
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);
        });


        // Movie
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.HasOne(e => e.Genre)
                  .WithMany(g => g.Movies)
                  .HasForeignKey(e => e.GenreId);
        });

        // UserMovie
        modelBuilder.Entity<UserMovie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.MovieId }).IsUnique();

            entity.HasOne(um => um.User)
                  .WithMany(u => u.Library)
                  .HasForeignKey(um => um.UserId);

            entity.HasOne(um => um.Movie)
                  .WithMany(m => m.UserMovies)
                  .HasForeignKey(um => um.MovieId);
        });

        // Viewing
        modelBuilder.Entity<Viewing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateViewed).IsRequired();

            entity.HasOne(v => v.UserMovie)
                  .WithMany(um => um.Viewings)
                  .HasForeignKey(v => v.UserMovieId);
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(r => r.Viewing)
                  .WithOne(v => v.Review)
                  .HasForeignKey<Review>(r => r.ViewingId);
        });
    }
}
