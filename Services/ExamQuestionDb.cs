using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamQuestionDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public ExamQuestionDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExamQuestion>> GetQuestionsByPartIdAsync(ulong partId)
    {
        var examQuestions = await _dbContext.ExamQuestions
            .Where(eq => eq.ExamPartId == partId)
            .ToListAsync();

        var questionIds = examQuestions.Select(eq => eq.QuestionId).ToList();

        var questions = await _dbContext.Questions
            .Where(q => questionIds.Contains(q.Id))
            .ToListAsync();
        
        var result = examQuestions.Select(eq => new ExamQuestion
        {
            ExamPartId = eq.ExamPartId,
            OrderIndex = eq.OrderIndex,
            Point = eq.Point,
            Question = questions.First(q => q.Id == eq.QuestionId)
        }).ToList();

        return result;
    }

    public async Task<bool> AddExamQuestion(ExamQuestion eq)
    {
        _dbContext.ExamQuestions.Add(eq);

        return await _dbContext.SaveChangesAsync() > 0;
    }
}