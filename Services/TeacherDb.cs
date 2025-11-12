using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TeacherDb : UserDb
{
    public TeacherDb(ZaloRevisionAppDbContext dbContext) : base(dbContext) { }

    private async Task<Teacher> GetTeacherByIdAsync(ulong id)
    {
        return await _dbContext.Teachers.FirstAsync(t => t.Id == id);
    }

    public async Task<TeacherDTO> GetTeacherDTOByIdAsync(ulong id)
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

    public async Task<bool> AddTeacher(User u, Teacher t)
    {
        if (!(await AddUser(u))) return false;

        t.Id = u.Id;
        _dbContext.Teachers.Add(t);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTeacher(User u, Teacher t)
    {
        if (!(await UpdateUser(u))) return false;

        var teacher = await GetTeacherByIdAsync(t.Id);

        teacher.SubjectId = t.SubjectId;
        teacher.Grades = t.Grades;
        teacher.Introduction = t.Introduction;

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> NullifyTeacher(ulong id)
    {
        if (!(await NullifyUser(id))) return false;

        var deleteTeacher = await GetTeacherByIdAsync(id);
        deleteTeacher.NullifyTeacher();
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
