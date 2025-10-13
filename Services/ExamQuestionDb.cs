using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamQuestionDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public ExamQuestionDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddExamQuestion(ExamQuestion eq)
    {
        _dbContext.ExamQuestions.Add(eq);

        return await _dbContext.SaveChangesAsync() > 0;
    }
}