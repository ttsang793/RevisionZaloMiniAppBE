using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public UserDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetUserByIdAsync(ulong id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> NullifyUser(ulong id)
    {
        var deleteUser = await GetUserByIdAsync(id);
        deleteUser.NullifyUser();
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
