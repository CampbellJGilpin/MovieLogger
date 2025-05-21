using AutoMapper;
using movielogger.api.models.requests.genres;
using movielogger.api.models.requests.library;
using movielogger.api.models.requests.movies;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.requests.users;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.library;
using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.reviews;
using movielogger.api.models.responses.users;
using movielogger.api.models.responses.viewings;
using movielogger.dal.dtos;

namespace movielogger.api.mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        // Request to DTO
        CreateMap<CreateGenreRequest, GenreDto>();
        CreateMap<UpdateGenreRequest, GenreDto>();
        CreateMap<CreateLibraryItemRequest, LibraryItemDto>();
        CreateMap<UpdateLibraryItemRequest, LibraryItemDto>();
        CreateMap<CreateMovieRequest, MovieDto>();
        CreateMap<UpdateMovieRequest, MovieDto>();
        CreateMap<CreateReviewRequest, ReviewDto>();
        CreateMap<UpdateReviewRequest, ReviewDto>();
        CreateMap<CreateUserRequest, UserDto>();
        CreateMap<UpdateUserRequest, UserDto>();
        CreateMap<CreateViewingRequest, ViewingDto>();
        CreateMap<UpdateViewingRequest, ViewingDto>();
        
        // DTO to Response
        CreateMap<GenreDto, GenreResponse>();
        CreateMap<LibraryItemDto, LibraryItemResponse>();
        CreateMap<LibraryDto, LibraryResponse>();
        CreateMap<MovieDto, MovieResponse>();
        CreateMap<ReviewDto, ReviewResponse>();
        CreateMap<UserDto, UserResponse>();
        CreateMap<ViewingDto, ViewingReponse>();
    }
}