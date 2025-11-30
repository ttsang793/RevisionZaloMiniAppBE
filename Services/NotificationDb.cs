using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class NotificationDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public NotificationDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UpdateStudentStatus(ulong studentId, string key, bool status)
    {
        var user = await _dbContext.Users.Where(u => u.Id == studentId).FirstAsync();
        bool isStudent = await _dbContext.Students.AnyAsync(s => s.Id == studentId);
        if (user == null || !isStudent) return false;

        switch (key)
        {
            case "all": user.Notification = [status, status, status, status]; break;
            case "following":
                user.Notification[1] = status;
                user.Notification[0] = user.Notification[1] && user.Notification[2] && user.Notification[3];
                break;
            case "manual":
                user.Notification[2] = status;
                user.Notification[0] = user.Notification[1] && user.Notification[2] && user.Notification[3];
                break;
            case "reply":
                user.Notification[3] = status;
                user.Notification[0] = user.Notification[1] && user.Notification[2] && user.Notification[3];
                break;
            default: return false;
        }

        _dbContext.Users.Update(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTeacherStatus(ulong teacherId, string key, bool status)
    {
        var user = await _dbContext.Users.Where(u => u.Id == teacherId).FirstAsync();
        bool isTeacher = await _dbContext.Teachers.AnyAsync(s => s.Id == teacherId);
        if (user == null || !isTeacher) return false;

        switch (key)
        {
            case "all": user.Notification = [status, status, status]; break;
            case "manual":
                user.Notification[1] = status;
                user.Notification[0] = user.Notification[1] && user.Notification[2];
                break;
            case "reply":
                user.Notification[2] = status;
                user.Notification[0] = user.Notification[1] && user.Notification[2];
                break;
            default: return false;
        }

        _dbContext.Users.Update(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
