using movielogger.dal.dtos;

namespace movielogger.services.interfaces;

public interface IMoviesService : IMovieQueryService, IMovieCommandService
{
    // This interface now combines query and command operations
    // but consumers can depend on the more specific interfaces if needed
}