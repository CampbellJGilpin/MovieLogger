using AutoMapper;
using movielogger.dal.entities;
using movielogger.dal.dtos;

namespace movielogger.services.mapping;

public class ServicesMappingProfile : Profile
{
    public ServicesMappingProfile()
    {
        CreateMap<Genre, GenreDto>().ReverseMap();
        CreateMap<Movie, MovieDto>().ReverseMap();
        CreateMap<Review, ReviewDto>().ReverseMap();
        CreateMap<Viewing, ViewingDto>().ReverseMap();
    }
}