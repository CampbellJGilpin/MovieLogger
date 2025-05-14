using AutoMapper;
using movielogger.api.models;
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
    }
}