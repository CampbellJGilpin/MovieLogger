using AutoMapper;
using movielogger.api.models;
using movielogger.dal.dtos;

namespace movielogger.api.mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateMovieRequest, MovieDto>();
        CreateMap<UpdateMovieRequest, MovieDto>();
        CreateMap<CreateGenreRequest, GenreDto>();
        CreateMap<UpdateGenreRequest, GenreDto>();
        CreateMap<CreateReviewRequest, ReviewDto>();
        CreateMap<UpdateReviewRequest, ReviewDto>();
        
        CreateMap<MovieDto, MovieResponse>();
        CreateMap<GenreDto, GenreResponse>();
    }
}