using AutoMapper;
using movielogger.dal;
using movielogger.services.interfaces;
using movielogger.services.services;

namespace movielogger.services.tests.services;

public class AccountsServiceTests : BaseServiceTest
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public AccountsServiceTests(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
}