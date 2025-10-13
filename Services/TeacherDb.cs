using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TeacherDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public TeacherDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetUserByIdAsync(ulong id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Teacher> GetTeacherByIdAsync(ulong id)
    {
        return await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> UpdateTeacher(TeacherDTO t)
    {
        var user = await GetUserByIdAsync((ulong)t.Id);
        var teacher = await GetTeacherByIdAsync((ulong)t.Id);

        user.Name = t.Name;
        teacher.SubjectId = t.SubjectId;
        teacher.Grades = t.Grades;
        teacher.Introduction = t.Introduction;

        return await _dbContext.SaveChangesAsync() > 0;
    }
}
