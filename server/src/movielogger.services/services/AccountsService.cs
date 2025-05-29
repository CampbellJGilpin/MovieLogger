using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    
    public async Task<bool> AuthenticateUserAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName == username);
        if (user == null) return false;

        // TODO check hashed password
        return true;
    }
}