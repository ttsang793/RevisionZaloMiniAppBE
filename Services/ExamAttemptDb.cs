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

    public async Task<ulong> GetLastIdAsync()
    {
        return (await DbContext.ExamAttempts.OrderByDescending(e => e.Id).ToListAsync())[0].Id;
    }

    public async Task<bool> AddExamAttempt(ExamAttempt examAttempt)
    {
        DbContext.ExamAttempts.Add(examAttempt);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddExamAttemptAnswer(ExamAttemptAnswer examAttemptAnswer)
    {
        DbContext.ExamAttemptAnswers.Add(examAttemptAnswer);
        return await DbContext.SaveChangesAsync() > 0;
    }
}
