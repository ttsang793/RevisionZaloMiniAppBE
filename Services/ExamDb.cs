using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamDb
{
    public ZaloRevisionAppDbContext DbContext { get; }

    public ExamDb(ZaloRevisionAppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<List<ExamReadDTO>> GetExamsAsync()
    {
        var result = await (from e in DbContext.Exams
                            join u in DbContext.Users
                            on e.TeacherId equals u.Id
                            join s in DbContext.Subjects
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
                                Status = e.Status
                            }).ToListAsync();

        return result;
    }

    public async Task<List<ExamReadDTO>> GetExamsByTeacherAsync(ulong teacherId)
    {
        var result = await (from e in DbContext.Exams
                            join u in DbContext.Users
                            on e.TeacherId equals u.Id
                            join s in DbContext.Subjects
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
                                Status = e.Status
                            }).ToListAsync();

        return result;
    }

    public async Task<ExamReadDTO> GetExamById(ulong id)
    {
        var result = await (from e in DbContext.Exams
                            join u in DbContext.Users
                            on e.TeacherId equals u.Id
                            join s in DbContext.Subjects
                            on e.SubjectId equals s.Id
                            where e.Id == id
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
                                Status = e.Status
                            }).FirstAsync();

        return result;
    }

    public async Task<bool> AddExam(Exam exam)
    {
        Console.WriteLine(exam.Title);
        DbContext.Exams.Add(exam);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateExam(Exam exam)
    {
        DbContext.Exams.Update(exam);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateExam(ulong id)
    {
        var updateExam = await DbContext.Exams.FirstAsync(e => e.Id == id);

        DbContext.Exams.Update(updateExam);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExam(ulong id)
    {
        var deleteExam = await DbContext.Exams.FirstAsync(e => e.Id == id);

        DbContext.Exams.Remove(deleteExam);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UnpublishExam(ulong id)
    {
        var unpublishExam = await GetExamById(id);
        unpublishExam.Status = 1;
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> PublishExam(ulong id)
    {
        var publishExam = await GetExamById(id);
        publishExam.Status = 3;
        return await DbContext.SaveChangesAsync() > 0;
    }
}
