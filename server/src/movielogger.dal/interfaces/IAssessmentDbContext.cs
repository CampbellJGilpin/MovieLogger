using Microsoft.EntityFrameworkCore;
using movielogger.dal.entities;

namespace movielogger.dal;

public interface IAssessmentDbContext
{
    DbSet<Movie> Movies { get; }
    DbSet<Genre> Genres { get; }
    DbSet<User> Users { get; }
    DbSet<UserMovie> UserMovies { get; }
    DbSet<UserMovieViewing> UserMovieViewings { get; }
    DbSet<Review> Reviews { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<EventTypeReference> EventTypeReferences { get; }
    DbSet<EntityTypeReference> EntityTypeReferences { get; }
    DbSet<List> Lists { get; }
    DbSet<ListMovie> ListMovies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}