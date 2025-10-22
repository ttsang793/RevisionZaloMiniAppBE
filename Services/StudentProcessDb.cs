using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class StudentProcessDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public StudentProcessDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExamReadDTO>> GetFavorite(ulong studentId)
    {
        var favoriteExams = await _dbContext.StudentProcesses
            .Where(sp => sp.StudentId == studentId).Select(sp => sp.FavoriteExams).FirstAsync();

        var result = await (from e in _dbContext.Exams
                            join u in _dbContext.Users
                            on e.TeacherId equals u.Id
                            join s in _dbContext.Subjects
                            on e.SubjectId equals s.Id
                            where favoriteExams.Contains(e.Id)
                            select new ExamReadDTO
                            {
                                Id = e.Id,
                                ExamType = e.ExamType,
                                DisplayType = e.DisplayType,
                                Title = e.Title,
                                Grade = e.Grade,
                                TimeLimit = e.TimeLimit,
                                EarlyTurnIn = e.EarlyTurnIn,
                                AllowShowScore = e.AllowShowScore,
                                AllowPartSwap = e.AllowPartSwap,
                                TeacherId = u.Id,
                                TeacherName = u.Name,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                State = e.State
                            }).ToListAsync();

        return result;
    }

    public async Task<bool> HandleFavorite(ulong examId, ulong studentId)
    {
        var student = await _dbContext.StudentProcesses.FirstOrDefaultAsync(sp => sp.StudentId == studentId);
        if (student == null) return false;

        bool isAvaiable = student.FavoriteExams.Contains(examId);

        if (isAvaiable) student.FavoriteExams.Remove(examId);
        else student.FavoriteExams.Add(examId);

        _dbContext.StudentProcesses.Update(student);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
