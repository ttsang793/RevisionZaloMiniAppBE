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
                                TeacherAvatar = u.Avatar,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                UpdatedAt = (e.Status < 3) ? e.UpdatedAt : null,
                                PublishedAt = (e.Status == 3) ? e.PublishedAt : null,
                                Status = e.Status
                            }).ToListAsync();

        return result;
    }

    public async Task<List<ExamReadDTO>> GetPublishExamsAsync()
    {
        var result = await (from e in DbContext.Exams
                            join u in DbContext.Users
                            on e.TeacherId equals u.Id
                            join s in DbContext.Subjects
                            on e.SubjectId equals s.Id
                            where e.Status == 3
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
                                TeacherAvatar = u.Avatar,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                PublishedAt = e.PublishedAt
                            }).ToListAsync();

        return result;
    }

    public async Task<List<ExamReadDTO>> GetPublishExamsByTeacherAsync(ulong teacherId)
    {
        var result = await (from e in DbContext.Exams
                            join u in DbContext.Users
                            on e.TeacherId equals u.Id
                            join s in DbContext.Subjects
                            on e.SubjectId equals s.Id
                            where e.TeacherId == teacherId && e.Status == 3
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
                                TeacherAvatar = u.Avatar,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                PublishedAt = e.PublishedAt
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
                                TeacherAvatar = u.Avatar,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                UpdatedAt = (e.Status < 3) ? e.UpdatedAt : null,
                                PublishedAt = (e.Status == 3) ? e.PublishedAt : null,
                                Status = e.Status
                            }).ToListAsync();

        return result;
    }

    public async Task<Exam> GetExamById(ulong id)
    {
        return await DbContext.Exams.FirstAsync(e => e.Id == id);
    }

    public async Task<ExamReadDTO> GetExamDTOById(ulong id)
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
                                TeacherAvatar = u.Avatar,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                UpdatedAt = (e.Status < 3) ? e.UpdatedAt : null,
                                PublishedAt = (e.Status == 3) ? e.PublishedAt : null,
                                Status = e.Status
                            }).FirstAsync();

        return result;
    }

    public async Task<object> GetNumberOfPartsAndQuestionsByExamId(ulong id)
    {
        var query = (from e in DbContext.Exams
                     join ep in DbContext.ExamParts on e.Id equals ep.ExamId
                     join eq in DbContext.ExamQuestions on ep.Id equals eq.ExamPartId
                     where e.Id == id group eq by eq.ExamPartId into g
                     select new
                     {
                         ExamPartId = g.Key,
                         Count = g.Select(eq => eq.OrderIndex).Distinct().Count()
                     });

        return new { PartCount = await query.CountAsync(), QuestionCount = await query.SumAsync(q => q.Count) };
    }

    public async Task<List<string>> GetTopicByExamId(ulong id)
    {
        return await (from e in DbContext.Exams
                      join ep in DbContext.ExamParts on e.Id equals ep.ExamId
                      join eq in DbContext.ExamQuestions on ep.Id equals eq.ExamPartId
                      join q in DbContext.Questions on eq.QuestionId equals q.Id
                      join t in DbContext.Topics on q.TopicId equals t.Id
                      where e.Id == id
                      select t.Name).Distinct().ToListAsync();
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

    public async Task<bool> UpdateExam(ulong id, byte status)
    {
        var updateExam = await GetExamById(id);
        updateExam.Status = status;

        DbContext.Exams.Update(updateExam);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExam(ulong id)
    {
        var deleteExam = await GetExamById(id);

        DbContext.Exams.Remove(deleteExam);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> PublishExam(ulong id)
    {
        var publishExam = await GetExamById(id);
        publishExam.PublishedAt = DateTime.Now;
        publishExam.Status = 3;
        return await DbContext.SaveChangesAsync() > 0;
    }
}
