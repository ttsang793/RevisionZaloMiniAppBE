using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserDb
{
    private protected ZaloRevisionAppDbContext _dbContext;

    public UserDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #pragma warning disable CS8603 // Possible null reference return.
    public async Task<User> GetUserByZaloId(string zaloId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.ZaloId == zaloId);
    }

    public async Task<User> GetUserByIdAsync(ulong id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    #pragma warning restore CS8603 // Possible null reference return.

    private protected async Task<bool> AddUser(User user)
    {
        await _dbContext.Users.AddAsync(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    private protected async Task<bool> UpdateUser(User user)
    {
        User updateUser = await GetUserByIdAsync(user.Id);
        updateUser.Name = user.Name;
        updateUser.Avatar = user.Avatar;
        updateUser.Email = user.Email;

        _dbContext.Users.Update(updateUser);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUserAvatar(ulong id, string url)
    {
        User updateUser = await GetUserByIdAsync(id);
        updateUser.Avatar = url;
        _dbContext.Users.Update(updateUser);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    private protected async Task<bool> NullifyUser(ulong id)
    {
        var deleteUser = await GetUserByIdAsync(id);
        deleteUser.NullifyUser();
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
