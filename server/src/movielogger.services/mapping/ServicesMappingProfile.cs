using AutoMapper;
using movielogger.dal.entities;
using movielogger.dal.dtos;

namespace movielogger.services.mapping;

public class ServicesMappingProfile : Profile
{
    public ServicesMappingProfile()
    {
        CreateMap<Genre, GenreDto>();

        CreateMap<GenreDto, Genre>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Movies, opt => opt.Ignore());

        CreateMap<Movie, MovieDto>()
            .ForMember(dest => dest.PosterPath, opt => opt.MapFrom(src => src.PosterPath));

        CreateMap<MovieDto, Movie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Genre, opt => opt.Ignore())
            .ForMember(dest => dest.UserMovies, opt => opt.Ignore())
            .ForMember(dest => dest.PosterPath, opt => opt.MapFrom(src => src.PosterPath));

        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.DateViewed, opt => opt.MapFrom(src => src.Viewing.DateViewed))
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Viewing.UserMovie.Movie.Title));

        CreateMap<ReviewDto, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ViewingId, opt => opt.Ignore());

        CreateMap<Viewing, ViewingDto>()
            .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.UserMovie.Movie))
            .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.UserMovie.MovieId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserMovie.UserId));
        CreateMap<ViewingDto, Viewing>();

        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<LibraryItemDto, UserMovie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Movie, opt => opt.Ignore())
            .ForMember(dest => dest.Viewings, opt => opt.Ignore())
            .ForMember(dest => dest.OwnsMovie, opt => opt.MapFrom(src => src.OwnsMovie))
            .ForMember(dest => dest.Favourite, opt => opt.MapFrom(src => src.Favourite))
            .ForMember(dest => dest.UpcomingViewDate, opt => opt.MapFrom((src, dest) => src.WatchLater ? DateTime.UtcNow.AddDays(1) : (DateTime?)null));

        CreateMap<UserMovie, LibraryItemDto>()
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Movie.ReleaseDate))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Movie.Genre.Title))
            .ForMember(dest => dest.Favourite, opt => opt.MapFrom(src => src.Favourite))
            .ForMember(dest => dest.InLibrary, opt => opt.MapFrom(src => src.OwnsMovie))
            .ForMember(dest => dest.WatchLater, opt => opt.MapFrom(src => src.UpcomingViewDate.HasValue))
            .ForMember(dest => dest.OwnsMovie, opt => opt.MapFrom(src => src.OwnsMovie));

        // List mappings
        CreateMap<List, ListDto>()
            .ForMember(dest => dest.MovieCount, opt => opt.MapFrom(src => src.ListMovies.Count(lm => !lm.Movie.IsDeleted)))
            .ForMember(dest => dest.Movies, opt => opt.MapFrom(src => src.ListMovies.Select(lm => lm.Movie)));

        CreateMap<ListDto, List>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ListMovies, opt => opt.Ignore());

        CreateMap<ListMovie, ListMovieDto>()
            .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie))
            .ForMember(dest => dest.List, opt => opt.MapFrom(src => src.List));

        CreateMap<ListMovieDto, ListMovie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Movie, opt => opt.Ignore())
            .ForMember(dest => dest.List, opt => opt.Ignore());
    }
}