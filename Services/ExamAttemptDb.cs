using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamAttemptDb
{
    public ZaloRevisionAppDbContext DbContext { get; }

    public ExamAttemptDb(ZaloRevisionAppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<ExamAttempt> GetLatestExamAttempt(ulong studentId, ulong examId)
    {
        var result = await (from ea in DbContext.ExamAttempts
                            join eaa in DbContext.ExamAttemptAnswers
                            on ea.Id equals eaa.ExamAttemptId
                            where ea.StudentId == studentId && ea.ExamId == examId
                            orderby ea.SubmittedAt descending
                            select new ExamAttempt
                            {
                                Score = ea.Score,
                                Comment = ea.Comment,
                                PartOrder = ea.PartOrder,
                                ExamAttemptAnswers = DbContext.ExamAttemptAnswers.Where(a => a.ExamAttemptId == ea.Id).ToList()
                            }).FirstAsync();

        return result;
    }

    public async Task<ExamAttempt?> AddExamAttempt(ExamAttempt examAttempt)
    {
        DbContext.ExamAttempts.Add(examAttempt);
        bool saved = await DbContext.SaveChangesAsync() > 0;
        return saved ? examAttempt : null;
    }

    public async Task<bool> AddExamAttemptAnswer(ExamAttemptAnswer examAttemptAnswer)
    {
        DbContext.ExamAttemptAnswers.Add(examAttemptAnswer);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveLastIndex()
    {
        DbContext.ExamAttempts.Remove((await DbContext.ExamAttempts.OrderByDescending(e => e.Id).ToListAsync())[0]);
        return await DbContext.SaveChangesAsync() > 0;
    }
}
