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
        
        CreateMap<Movie, MovieDto>();

        CreateMap<MovieDto, Movie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Genre, opt => opt.Ignore()) 
            .ForMember(dest => dest.UserMovies, opt => opt.Ignore()); 

        
        CreateMap<Review, ReviewDto>().ReverseMap();
        
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
            .ForMember(dest => dest.Viewings, opt => opt.Ignore());

        CreateMap<UserMovie, LibraryItemDto>();
    }
}