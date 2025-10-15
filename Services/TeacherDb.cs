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

    public async Task<TeacherDTO> GetTeacherByIdAsync(ulong id)
    {
        var result = await (from t in _dbContext.Teachers
                            join u in _dbContext.Users
                            on t.Id equals u.Id
                            where t.Id == id
                            select new TeacherDTO
                            {
                                Id = id,
                                Name = u.Name,
                                Role = u.Role,
                                SubjectId = t.SubjectId,
                                Grades = t.Grades,
                                Introduction = t.Introduction
                            }).FirstAsync();

        return result;
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
