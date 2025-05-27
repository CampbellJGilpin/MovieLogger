using Microsoft.EntityFrameworkCore;
using movielogger.dal.entities;

namespace movielogger.dal;

public interface IAssessmentDbContext
{
    DbSet<Movie> Movies { get; }
    DbSet<Genre> Genres { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}