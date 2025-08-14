using Microsoft.EntityFrameworkCore;
using movielogger.dal.entities;

namespace movielogger.dal;

public class MovieLoggerDbContext : DbContext, IAssessmentDbContext
{
    public MovieLoggerDbContext(DbContextOptions<MovieLoggerDbContext> options)
        : base(options) { }

    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<Movie> Movies => Set<Movie>();
    public virtual DbSet<Genre> Genres => Set<Genre>();
    public virtual DbSet<UserMovie> UserMovies => Set<UserMovie>();
    public virtual DbSet<Viewing> Viewings => Set<Viewing>();
    public virtual DbSet<Review> Reviews => Set<Review>();
    public virtual DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public virtual DbSet<EventTypeReference> EventTypeReferences => Set<EventTypeReference>();
    public virtual DbSet<EntityTypeReference> EntityTypeReferences => Set<EntityTypeReference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("public");

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

        // EventTypeReference
        modelBuilder.Entity<EventTypeReference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EventType).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // EntityTypeReference
        modelBuilder.Entity<EntityTypeReference>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EntityType).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.EventType).IsRequired().HasConversion<int>();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.AdditionalData).HasMaxLength(1000);
            entity.Property(e => e.EntityType).HasMaxLength(50);

            // Index for efficient querying
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.UserId);

            // Foreign key relationship to EventTypeReference
            entity.HasOne(a => a.EventTypeReference)
                  .WithMany(et => et.AuditLogs)
                  .HasForeignKey(a => a.EventType)
                  .HasPrincipalKey(et => et.EventType)
                  .OnDelete(DeleteBehavior.Restrict);

            // Foreign key relationship to EntityTypeReference
            entity.HasOne(a => a.EntityTypeReference)
                  .WithMany(et => et.AuditLogs)
                  .HasForeignKey(a => a.EntityType)
                  .HasPrincipalKey(et => et.EntityType)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
