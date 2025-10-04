using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AchievementDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public AchievementDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Achievement>> GetAsync()
    {
        return await _dbContext.Achivements.ToListAsync();
    }
}
