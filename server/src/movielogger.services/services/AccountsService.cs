using AutoMapper;
using movielogger.dal;
using movielogger.services.interfaces;

namespace movielogger.services.services;

public class AccountsService : IAccountsService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;

    public AccountsService(IAssessmentDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public Task<bool> AuthenticateUserAsync(string username, string password)
    {
        throw new NotImplementedException();
    }
}