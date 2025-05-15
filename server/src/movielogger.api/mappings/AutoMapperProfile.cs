using AutoMapper;
using movielogger.api.models.requests.genres;
using movielogger.api.models.requests.library;
using movielogger.api.models.requests.movies;
using movielogger.api.models.requests.reviews;
using movielogger.api.models.requests.viewings;
using movielogger.api.models.responses.genres;
using movielogger.api.models.responses.library;
using movielogger.api.models.responses.movies;
using movielogger.api.models.responses.reviews;
using movielogger.api.models.responses.viewings;
using movielogger.dal.dtos;

namespace movielogger.api.mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateGenreRequest, GenreDto>();
        CreateMap<UpdateGenreRequest, GenreDto>();
        CreateMap<CreateMovieRequest, MovieDto>();
        CreateMap<UpdateMovieRequest, MovieDto>();
        CreateMap<CreateReviewRequest, ReviewDto>();
        CreateMap<UpdateReviewRequest, ReviewDto>();
        CreateMap<CreateViewingRequest, ViewingDto>();
        CreateMap<UpdateViewingRequest, ViewingDto>();
        
        CreateMap<GenreDto, GenreResponse>();
        CreateMap<MovieDto, MovieResponse>();
        CreateMap<ReviewDto, ReviewResponse>();
        CreateMap<ViewingDto, ViewingReponse>();
        CreateMap<LibraryDto, LibraryResponse>();
        CreateMap<LibraryItemDto, LibraryItemResponse>();
    }
}