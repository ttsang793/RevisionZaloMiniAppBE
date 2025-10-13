using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public ExamDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExamReadDTO>> GetExamsAsync()
    {
        var result = await (from e in _dbContext.Exams
                            join u in _dbContext.Users
                            on e.TeacherId equals u.Id
                            join s in _dbContext.Subjects
                            on e.SubjectId equals s.Id
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

    public async Task<List<ExamReadDTO>> GetExamsByTeacherAsync(ulong teacherId)
    {

        var result = await (from e in _dbContext.Exams
                            join u in _dbContext.Users
                            on e.TeacherId equals u.Id
                            join s in _dbContext.Subjects
                            on e.SubjectId equals s.Id
                            where e.TeacherId == teacherId
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

    public async Task<Exam> GetExamById(ulong id)
    {
        return await _dbContext.Exams.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> AddExam(Exam exam)
    {
        Console.WriteLine(exam.Title);
        _dbContext.Exams.Add(exam);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateExam(Exam exam)
    {
        _dbContext.Exams.Update(exam);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExam(ulong id)
    {
        var deleteExam = await GetExamById(id);

        _dbContext.Exams.Remove(deleteExam);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UnpublishExam(ulong id)
    {
        var unpublishExam = await GetExamById(id);
        unpublishExam.State = 1;
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> PublishExam(ulong id)
    {
        var publishExam = await GetExamById(id);
        publishExam.State = 2;
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
