using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class StatisticDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public StatisticDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MonthlyAvgPointDTO>> GetAvgPointMonthly(ulong studentId, string subjectId)
    {
        int currentYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;

        var result = await (from ea in _dbContext.ExamAttempts
                            join e in _dbContext.Exams on ea.ExamId equals e.Id
                            where ea.StudentId == studentId && e.SubjectId == subjectId
                            group ea by new { ea.SubmittedAt.Month, ea.SubmittedAt.Year } into g
                            select new MonthlyAvgPointDTO
                            {
                                Month = g.Key.Month,
                                Year = g.Key.Year,
                                AvgPoint = g.Average(x => x.TotalPoint)
                            }).ToListAsync();

        return result;
    }

    public async Task<List<PointDifficultyDTO>> GetCorrectRate(ulong studentId, string subjectId)
    {
        var result = await (from eaa in _dbContext.ExamAttemptAnswers
                            join ea in _dbContext.ExamAttempts on eaa.ExamAttemptId equals ea.ExamId
                            join eq in _dbContext.ExamQuestions on eaa.ExamQuestionId equals eq.Id
                            join q in _dbContext.Questions on eq.QuestionId equals q.Id
                            join t in _dbContext.Topics on q.TopicId equals t.Id
                            where ea.StudentId == studentId && t.SubjectId == subjectId
                            group eaa by q.Difficulty into g
                            select new PointDifficultyDTO
                            {
                                Difficuly = g.Key,
                                AvgPoint = g.Average(eaa => eaa.Point)
                            }).ToListAsync();

        return result;
    }

    public async Task<List<ExamReadDTO>> GetBestResultExam(ulong studentId, string subjectId)
    {
        var result = await (from e in _dbContext.Exams
                            join ea in _dbContext.ExamAttempts
                            on e.Id equals ea.ExamId
                            join u in _dbContext.Users
                            on e.TeacherId equals u.Id
                            join s in _dbContext.Subjects
                            on e.SubjectId equals s.Id
                            where ea.StudentId == studentId && s.Id == subjectId
                            group ea by new
                            {
                                e.Id, e.ExamType, e.DisplayType, e.Title,
                                e.Grade, e.TimeLimit, e.EarlyTurnIn, e.AllowShowScore, e.AllowPartSwap,
                                TeacherId = u.Id,
                                TeacherName = u.Name,
                                TeacherAvatar = u.Avatar,
                                SubjectId = s.Id,
                                SubjectName = s.Name,
                                e.PublishedAt
                            } into g
                            select new ExamReadDTO
                            {
                                Id = g.Key.Id,
                                ExamType = g.Key.ExamType,
                                DisplayType = g.Key.DisplayType,
                                Title = g.Key.Title,
                                Grade = g.Key.Grade,
                                TimeLimit = g.Key.TimeLimit,
                                EarlyTurnIn = g.Key.EarlyTurnIn,
                                AllowShowScore = g.Key.AllowShowScore,
                                AllowPartSwap = g.Key.AllowPartSwap,
                                TeacherId = g.Key.TeacherId,
                                TeacherName = g.Key.TeacherName,
                                TeacherAvatar = g.Key.TeacherAvatar,
                                SubjectId = g.Key.SubjectId,
                                SubjectName = g.Key.SubjectName,
                                PublishedAt = g.Key.PublishedAt,
                                TotalPoint = g.Max(x => x.TotalPoint)
                            }).ToListAsync();

        return result;
    }

    public async Task<List<ExamReadDTO>> GetWorstResultExam(ulong studentId, string subjectId)
    {
        var query = await (from e in _dbContext.Exams
                           join ea in _dbContext.ExamAttempts
                           on e.Id equals ea.ExamId
                           join u in _dbContext.Users
                           on e.TeacherId equals u.Id
                           join s in _dbContext.Subjects
                           on e.SubjectId equals s.Id
                           where ea.StudentId == studentId && s.Id == subjectId
                           orderby ea.SubmittedAt descending
                           orderby ea.TotalPoint ascending
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
                               PublishedAt = e.PublishedAt,
                               TotalPoint = ea.TotalPoint
                           }).ToListAsync();

        var minPoint = query[0].TotalPoint;
        List<ExamReadDTO> result = [];

        for (int i = 0; i < query.Count; i++)
        {
            if (query[i].TotalPoint > minPoint) break;
            else result.Add(query[i]);
        }

        return result;
    }
}