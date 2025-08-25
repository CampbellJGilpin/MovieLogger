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
    public virtual DbSet<UserMovieViewing> UserMovieViewings => Set<UserMovieViewing>();
    public virtual DbSet<Review> Reviews => Set<Review>();
    public virtual DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public virtual DbSet<EventTypeReference> EventTypeReferences => Set<EventTypeReference>();
    public virtual DbSet<EntityTypeReference> EntityTypeReferences => Set<EntityTypeReference>();
    public virtual DbSet<List> Lists => Set<List>();
    public virtual DbSet<ListMovie> ListMovies => Set<ListMovie>();

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

        // UserMovieViewing
        modelBuilder.Entity<UserMovieViewing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DateViewed).IsRequired();

            entity.HasOne(v => v.User)
                  .WithMany(u => u.UserMovieViewings)
                  .HasForeignKey(v => v.UserId);

            entity.HasOne(v => v.Movie)
                  .WithMany(m => m.UserMovieViewings)
                  .HasForeignKey(v => v.MovieId);
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(r => r.UserMovieViewing)
                  .WithOne(v => v.Review)
                  .HasForeignKey<Review>(r => r.UserMovieViewingId);
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

        // List
        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
            entity.HasIndex(e => e.UserId);

            entity.HasOne(l => l.User)
                  .WithMany(u => u.Lists)
                  .HasForeignKey(l => l.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ListMovie
        modelBuilder.Entity<ListMovie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ListId, e.MovieId }).IsUnique();
            entity.HasIndex(e => e.ListId);
            entity.HasIndex(e => e.MovieId);

            entity.HasOne(lm => lm.List)
                  .WithMany(l => l.ListMovies)
                  .HasForeignKey(lm => lm.ListId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(lm => lm.Movie)
                  .WithMany(m => m.ListMovies)
                  .HasForeignKey(lm => lm.MovieId)
                  .OnDelete(DeleteBehavior.Cascade);
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
