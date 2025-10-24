using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public UserDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ALL ROLES
    public async Task<User> GetUserByIdAsync(ulong id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> UpdateUser(User user, ulong id)
    {
        User updateUser = await GetUserByIdAsync(id);
        updateUser.Name = user.Name;
        updateUser.Avatar = user.Avatar;

        _dbContext.Users.Update(updateUser);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> NullifyUser(ulong id)
    {
        var deleteUser = await GetUserByIdAsync(id);
        deleteUser.NullifyUser();
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // STUDENT
    public async Task<bool> AddStudent(User user)
    {
        await _dbContext.Users.AddAsync(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateStudentName(ulong id, string name)
    {
        var updateUser = await GetUserByIdAsync(id);
        updateUser.Name = name;
        _dbContext.Users.Update(updateUser);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // TEACHER
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
        _dbContext.Users.Add(u);
        if (await _dbContext.SaveChangesAsync() <= 0) return false;

        t.Id = u.Id;
        _dbContext.Teachers.Add(t);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTeacher(User u, Teacher t, ulong id)
    {
        if (!(await UpdateUser(u, id))) return false;

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
