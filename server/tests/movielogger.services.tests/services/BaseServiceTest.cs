using AutoFixture;
using AutoMapper;
using movielogger.dal;
using movielogger.services.mapping;
using NSubstitute;

namespace movielogger.services.tests.services;

public abstract class BaseServiceTest
{
    protected readonly IFixture Fixture;
    protected readonly IAssessmentDbContext _dbContext;
    protected readonly IMapper _mapper;

    protected BaseServiceTest()
    {
        Fixture = new Fixture();
        Fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _dbContext = Substitute.For<IAssessmentDbContext>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ServicesMappingProfile>();
        });

        _mapper = config.CreateMapper();
    }
}