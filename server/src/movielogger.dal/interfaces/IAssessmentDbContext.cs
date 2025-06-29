using Microsoft.EntityFrameworkCore;
using movielogger.dal.entities;

namespace movielogger.dal;

public interface IAssessmentDbContext
{
    DbSet<Movie> Movies { get; }
    DbSet<Genre> Genres { get; }
    DbSet<User> Users { get; }
    DbSet<UserMovie> UserMovies { get; }
    DbSet<Viewing> Viewings { get; }
    DbSet<Review> Reviews { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<EventTypeReference> EventTypeReferences { get; }
    DbSet<EntityTypeReference> EntityTypeReferences { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}