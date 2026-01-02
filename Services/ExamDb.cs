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

    public async Task<List<ExamReadDTO>> GetPublishExamsAsync(string? search, string? subject, sbyte? grade, string? type)
    {
        var result = await (from e in DbContext.Exams
                            join u in DbContext.Users
                            on e.TeacherId equals u.Id
                            join s in DbContext.Subjects
                            on e.SubjectId equals s.Id
                            where e.Status == 3
                                && (string.IsNullOrEmpty(subject) || e.SubjectId == subject)
                                && (grade == null || grade == -1 || e.Grade == grade)
                                && (string.IsNullOrEmpty(type) || e.ExamType == type)
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

        return result.Where(e => string.IsNullOrEmpty(search) || e.Title.Contains(search)).ToList();
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
                                AllowQuestionSwap = e.AllowQuestionSwap,
                                AllowAnswerSwap = e.AllowAnswerSwap,
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
                                AllowQuestionSwap = e.AllowQuestionSwap,
                                AllowAnswerSwap = e.AllowAnswerSwap,
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

    public async Task<ExamDetailDTO> GetExamDetailById(ulong id)
    {
        var type = (await GetExamById(id)).DisplayType;

        if (type == "pdf")
        {
            var code = await (from e in DbContext.Exams
                              join pec in DbContext.PdfExamCodes on e.Id equals pec.ExamId
                              where e.Id == id
                              select new
                              {
                                  PartId = pec.Id,
                                  PartCount = pec.NumPart
                              }).FirstOrDefaultAsync();

            var query = await (from peq in DbContext.PdfExamCodeQuestions
                               where peq.PdfExamCodeId == code.PartId
                               select peq).CountAsync();

            return new ExamDetailDTO
            {
                PartCount = code.PartCount,
                QuestionCount = query
            };
        }

        var countQuery = (from e in DbContext.Exams
                          join ep in DbContext.ExamParts on e.Id equals ep.ExamId
                          join eq in DbContext.ExamQuestions on ep.Id equals eq.ExamPartId
                          where e.Id == id group eq by eq.ExamPartId into g
                          select new
                          {
                              ExamPartId = g.Key,
                              Count = g.Select(eq => eq.OrderIndex).Distinct().Count()
                          });

        var topicList = await (from e in DbContext.Exams
                               join ep in DbContext.ExamParts on e.Id equals ep.ExamId
                               join eq in DbContext.ExamQuestions on ep.Id equals eq.ExamPartId
                               join q in DbContext.Questions on eq.QuestionId equals q.Id
                               join t in DbContext.Topics on q.TopicId equals t.Id
                               where e.Id == id
                               select t.Name).Distinct().ToListAsync();

        return new ExamDetailDTO
        {
            PartCount = await countQuery.CountAsync(),
            QuestionCount = await countQuery.SumAsync(q => q.Count),
            Topics = topicList
        };
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
