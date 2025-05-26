using AutoMapper;
using Microsoft.EntityFrameworkCore;
using movielogger.dal;
using NSubstitute;

namespace movielogger.services.tests.services;

public abstract class BaseServiceTest
{
    protected readonly AssessmentDbContext DbContext;
    protected readonly IMapper Mapper;

    protected BaseServiceTest()
    {
        // Substitute the DbContext (using in-memory options)
        var options = new DbContextOptionsBuilder<AssessmentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new AssessmentDbContext(options);

        // Substitute IMapper unless you need real mapping
        Mapper = Substitute.For<IMapper>();
    }
}