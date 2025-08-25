using AutoMapper;
using movielogger.api.models.requests.genres;
using movielogger.api.models.requests.library;
using movielogger.api.models.requests.lists;
using movielogger.api.models.requests.movies;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.requests.users;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.library;
using movielogger.api.models.responses.lists;
using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.reviews;
using movielogger.api.models.responses.users;
using movielogger.api.models.responses.viewings;
using movielogger.dal.dtos;
using movielogger.dal.entities;

namespace movielogger.api.mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // Request to DTO
        CreateMap<CreateGenreRequest, GenreDto>();
        CreateMap<UpdateGenreRequest, GenreDto>();
        CreateMap<CreateLibraryItemRequest, LibraryItemDto>()
            .ForMember(dest => dest.Favourite, opt => opt.MapFrom(src => src.IsFavorite));
        CreateMap<UpdateLibraryItemRequest, LibraryItemDto>()
            .ForMember(dest => dest.Favourite, opt => opt.MapFrom(src => src.IsFavorite));
        CreateMap<CreateMovieRequest, MovieDto>()
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => new GenreDto { Id = src.GenreId }));
        CreateMap<UpdateMovieRequest, MovieDto>()
            .ForMember(dest => dest.Genre, opt => opt.Ignore());
        CreateMap<CreateReviewRequest, ReviewDto>();
        CreateMap<UpdateReviewRequest, ReviewDto>();
        CreateMap<CreateUserRequest, UserDto>();
        CreateMap<UpdateUserRequest, UserDto>();
        CreateMap<UpdateViewingRequest, ViewingDto>();

        CreateMap<CreateViewingRequest, ViewingDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.UserMovieId, opt => opt.Ignore());


        // DTO to Response
        CreateMap<GenreDto, GenreResponse>();
        CreateMap<LibraryItemDto, LibraryItemResponse>()
            .ForMember(dest => dest.Favourite, opt => opt.MapFrom(src => src.Favourite.ToString().ToLower()))
            .ForMember(dest => dest.InLibrary, opt => opt.MapFrom(src => src.InLibrary.ToString().ToLower()))
            .ForMember(dest => dest.WatchLater, opt => opt.MapFrom(src => src.WatchLater.ToString().ToLower()));
        CreateMap<LibraryDto, LibraryResponse>();
        CreateMap<MovieDto, MovieResponse>();
        CreateMap<ReviewDto, ReviewResponse>();
        CreateMap<UserDto, UserResponse>();

        CreateMap<ViewingDto, ViewingResponse>()
            .ForMember(dest => dest.ViewingId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UpcomingViewDate, opt => opt.MapFrom(src => src.DateViewed))
            .ForMember(dest => dest.DateViewed, opt => opt.MapFrom(src => src.DateViewed))
            .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.Review));

        // User mappings
        CreateMap<User, UserResponse>();
        CreateMap<UserDto, UserResponse>();

        CreateMap<UserMovieDto, UserMovieResponse>()
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => new GenreResponse { Id = src.GenreId, Title = src.GenreTitle }));

        // Genre Preferences mappings
        CreateMap<GenrePreferenceDto, GenrePreferenceResponse>();
        CreateMap<GenrePreferencesSummaryDto, GenrePreferencesSummaryResponse>();

        // List mappings
        CreateMap<CreateListRequest, ListDto>();
        CreateMap<UpdateListRequest, ListDto>();
        CreateMap<ListDto, ListResponse>();
        CreateMap<ListDto, ListSummaryResponse>();
        CreateMap<ListMovieDto, ListResponse>();
    }
}