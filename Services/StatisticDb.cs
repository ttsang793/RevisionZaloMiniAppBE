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

    public async Task<MonthlyAvgPointDTO> GetAvgPointMonthly(ulong studentId, string subjectId)
    {
        int currentYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;

        var query = await (from ea in _dbContext.ExamAttempts
                           join e in _dbContext.Exams on ea.ExamId equals e.Id
                           where ea.StudentId == studentId && e.SubjectId == subjectId && ea.SubmittedAt >= new DateTime(currentYear, 7, 1) && ea.SubmittedAt < new DateTime(currentYear + 1, 7, 1)
                           group ea by new { ea.SubmittedAt.Month, ea.SubmittedAt.Year } into g
                           select new
                           {
                               g.Key.Month, g.Key.Year,
                               AvgPoint = (decimal)g.Average(x => x.TotalPoint)!
                           }).ToListAsync();

        MonthlyAvgPointDTO result = new MonthlyAvgPointDTO();
        int month = 7;
        while (result.Time.Count < 12)
        {
            var r = query.FirstOrDefault(q => q.Month == month && q.Year == currentYear);
            if (r != null) result.AddMonth(month, currentYear, r.AvgPoint);
            else result.AddMonth(month, currentYear);

            month++;
            if (month > 12)
            {
                month = 1;
                currentYear++;
            }
        }

        return result;
    }

    public async Task<List<PointDifficultyDTO>> GetCorrectRate(ulong studentId, string subjectId)
    {
        int currentYear = DateTime.Now.Month >= 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;

        var correctQuery = await (from eaa in _dbContext.ExamAttemptAnswers
                                  join ea in _dbContext.ExamAttempts on eaa.ExamAttemptId equals ea.ExamId
                                  join eq in _dbContext.ExamQuestions on eaa.ExamQuestionId equals eq.Id
                                  join q in _dbContext.Questions on eq.QuestionId equals q.Id
                                  join t in _dbContext.Topics on q.TopicId equals t.Id
                                  where ea.StudentId == studentId && t.SubjectId == subjectId
                                      && ea.SubmittedAt >= new DateTime(currentYear, 7, 1) && ea.SubmittedAt < new DateTime(currentYear + 1, 7, 1)
                                  select new { q.Difficulty, q.Type, eaa.Correct }
                                 ).ToListAsync();

        var difficultyList = await (from eaa in _dbContext.ExamAttemptAnswers
                                    join ea in _dbContext.ExamAttempts on eaa.ExamAttemptId equals ea.ExamId
                                    join eq in _dbContext.ExamQuestions on eaa.ExamQuestionId equals eq.Id
                                    join q in _dbContext.Questions on eq.QuestionId equals q.Id
                                    join t in _dbContext.Topics on q.TopicId equals t.Id
                                    where ea.StudentId == studentId && t.SubjectId == subjectId
                                        && ea.SubmittedAt >= new DateTime(currentYear, 7, 1) && ea.SubmittedAt < new DateTime(currentYear + 1, 7, 1)
                                    group eaa by new { q.Difficulty, q.Type } into g
                                    orderby g.Key.Difficulty ascending
                                    select new PointDifficultyItemDTO
                                    {
                                        Difficulty = g.Key.Difficulty,
                                        Type = g.Key.Type,
                                    }).ToListAsync();

        foreach (var row in correctQuery)
        {
            int index = difficultyList.FindIndex(q => q.Difficulty == row.Difficulty && q.Type == row.Type);

            if (row.Type != "sorting" && row.Type != "true-false-thpt")
            {
                difficultyList[index].Total += 1;
                if (row.Correct.ElementAt(0) == 1) difficultyList[index].Count += 1;
            }
            else
            {
                foreach (var c in row.Correct)
                {
                    difficultyList[index].Total += 1;
                    if (c == 1) difficultyList[index].Count += 1;
                }
            }

            difficultyList[index].Percentage = difficultyList[index].Count * (decimal)100 / difficultyList[index].Total;
        }

        List<PointDifficultyDTO> result = new List<PointDifficultyDTO>();
        for (byte i = 1; i <= 4; i++)
        {
            PointDifficultyDTO row = new PointDifficultyDTO(i);
            int correct = 0;
            int total = 0;

            foreach (var item in difficultyList.FindAll(d => d.Difficulty == i))
            {
                row.Types.Add(item);
                correct += item.Count;
                total += item.Total;
            }

            row.Percentage = (total > 0) ? correct * (decimal)100 / total : 0;
            result.Add(row);
        }

        return result;
    }

    public async Task<List<ExamReadStatDTO>> GetBestResultExam(ulong studentId, string subjectId)
    {
        try
        {
            var query = await (from e in _dbContext.Exams
                               join ea in _dbContext.ExamAttempts
                               on e.Id equals ea.ExamId
                               join u in _dbContext.Users
                               on e.TeacherId equals u.Id
                               join s in _dbContext.Subjects
                               on e.SubjectId equals s.Id
                               where ea.StudentId == studentId && s.Id == subjectId && ea.IsPractice == false
                               orderby ea.SubmittedAt descending
                               orderby ea.TotalPoint descending
                               select new ExamReadStatDTO
                               {
                                   Id = e.Id,
                                   ExamType = e.ExamType,
                                   DisplayType = e.DisplayType,
                                   Title = e.Title,
                                   Grade = e.Grade,
                                   TimeLimit = e.TimeLimit,
                                   TeacherId = u.Id,
                                   TeacherName = u.Name,
                                   TeacherAvatar = u.Avatar,
                                   SubjectId = s.Id,
                                   SubjectName = s.Name,
                                   PublishedAt = e.PublishedAt,
                                   TotalPoint = ea.TotalPoint,
                                   AttemptId = ea.Id,
                                   SubmittedAt = ea.SubmittedAt
                               }).ToListAsync();

            var maxPoint = query[0].TotalPoint;
            List<ExamReadStatDTO> result = [];

            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].TotalPoint < maxPoint) break;
                else result.Add(query[i]);
            }

            return result;
        }
        catch
        {
            return [];
        }
    }

    public async Task<List<ExamReadStatDTO>> GetWorstResultExam(ulong studentId, string subjectId)
    {
        try
        {
            var query = await (from e in _dbContext.Exams
                               join ea in _dbContext.ExamAttempts
                               on e.Id equals ea.ExamId
                               join u in _dbContext.Users
                               on e.TeacherId equals u.Id
                               join s in _dbContext.Subjects
                               on e.SubjectId equals s.Id
                               where ea.StudentId == studentId && s.Id == subjectId && ea.IsPractice == false
                               orderby ea.SubmittedAt descending
                               orderby ea.TotalPoint ascending
                               select new ExamReadStatDTO
                               {
                                   Id = e.Id,
                                   ExamType = e.ExamType,
                                   DisplayType = e.DisplayType,
                                   Title = e.Title,
                                   Grade = e.Grade,
                                   TimeLimit = e.TimeLimit,
                                   TeacherId = u.Id,
                                   TeacherName = u.Name,
                                   TeacherAvatar = u.Avatar,
                                   SubjectId = s.Id,
                                   SubjectName = s.Name,
                                   PublishedAt = e.PublishedAt,
                                   TotalPoint = ea.TotalPoint,
                                   AttemptId = ea.Id,
                                   SubmittedAt = ea.SubmittedAt
                               }).ToListAsync();

            var minPoint = query[0].TotalPoint;
            List<ExamReadStatDTO> result = [];

            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].TotalPoint > minPoint) break;
                else result.Add(query[i]);
            }

            return result;
        }
        catch
        {
            return [];
        }
    }
}