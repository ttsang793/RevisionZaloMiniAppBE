using backend.Models;

namespace backend.Services;

public class StudentDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public StudentDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddStudent(User user)
    {
        _dbContext.Users.Add(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}