using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using movielogger.dal.dtos;
using movielogger.dal.entities;
using movielogger.services.mapping;
using NSubstitute;

namespace movielogger.services.tests.services;

public abstract class BaseServiceTest
{
    protected readonly IAssessmentDbContext _dbContext;
    protected readonly IMapper _mapper;

    protected BaseServiceTest()
    {
        _dbContext = Substitute.For<IAssessmentDbContext>();
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServicesMappingProfile>();
        });

        _mapper = config.CreateMapper();
    }
}