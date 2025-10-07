using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class QuestionDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public QuestionDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Question>> GetAllQuestionsByTeacher()
    {
        return await _dbContext.Questions.ToListAsync();
    }

    public async Task<Question> GetQuestionById(ulong id)
    {
        return await _dbContext.Questions.FirstAsync(q => q.Id == id);
    }

    public async Task<MultipleChoiceQuestion> GetMultipleChoiceQuestionById(ulong id)
    {
        return await _dbContext.MultipleChoiceQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<TrueFalseQuestion> GetTrueFalseQuestionById(ulong id)
    {
        return await _dbContext.TrueFalseQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<ShortAnswerQuestion> GetShortAnswerQuestionById(ulong id)
    {
        return await _dbContext.ShortAnswerQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<FillInTheBlankQuestion> GetFillInTheBlankQuestionById(ulong id)
    {
        return await _dbContext.FillInTheBlankQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<ConstructedResponseQuestion> GetConstructedResponseQuestionById(ulong id)
    {
        return await _dbContext.ConstructedResponseQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<SortingQuestion> GetSortingQuestionById(ulong id)
    {
        return await _dbContext.SortingQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<ulong> GetLastIndex()
    {
        return (await _dbContext.Questions.OrderBy(s => s.Id).LastAsync()).Id;
    }

    public async Task<bool> AddMultipleChoiceQuestion(Question q, MultipleChoiceQuestion mcq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            mcq.Id = await GetLastIndex();
            _dbContext.MultipleChoiceQuestions.Add(mcq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddTrueFalseQuestion(Question q, TrueFalseQuestion tfq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            tfq.Id = await GetLastIndex();
            _dbContext.TrueFalseQuestions.Add(tfq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddShortAnswerQuestion(Question q, ShortAnswerQuestion saq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            saq.Id = await GetLastIndex();
            _dbContext.ShortAnswerQuestions.Add(saq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddFillInTheBlankQuestion(Question q, FillInTheBlankQuestion fitbq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            fitbq.Id = await GetLastIndex();
            _dbContext.FillInTheBlankQuestions.Add(fitbq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddConstructedResponseQuestion(Question q, ConstructedResponseQuestion crq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            crq.Id = await GetLastIndex();
            _dbContext.ConstructedResponseQuestions.Add(crq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateMultipleChoiceQuestion(Question q, MultipleChoiceQuestion mcq)
    {
        var existingQuestion = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == q.Id);
        if (existingQuestion == null) return false;

        q.CreatedAt = existingQuestion.CreatedAt;
        q.UpdatedAt = DateTime.Now;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Entry(q).State = EntityState.Modified;
            _dbContext.Entry(mcq).State = EntityState.Modified;

            var result = await _dbContext.SaveChangesAsync() > 0;
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateTrueFalseQuestion(Question q, TrueFalseQuestion tfq)
    {
        var existingQuestion = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == q.Id);
        if (existingQuestion == null) return false;

        q.CreatedAt = existingQuestion.CreatedAt;
        q.UpdatedAt = DateTime.Now;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Entry(q).State = EntityState.Modified;
            _dbContext.Entry(tfq).State = EntityState.Modified;

            var result = await _dbContext.SaveChangesAsync() > 0;
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateShortAnswerQuestion(Question q, ShortAnswerQuestion saq)
    {
        var existingQuestion = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == q.Id);
        if (existingQuestion == null) return false;

        q.CreatedAt = existingQuestion.CreatedAt;
        q.UpdatedAt = DateTime.Now;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Entry(q).State = EntityState.Modified;
            _dbContext.Entry(saq).State = EntityState.Modified;

            var result = await _dbContext.SaveChangesAsync() > 0;
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateFillInTheBlankQuestion(Question q, FillInTheBlankQuestion fitbq)
    {
        var existingQuestion = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == q.Id);
        if (existingQuestion == null) return false;

        q.CreatedAt = existingQuestion.CreatedAt;
        q.UpdatedAt = DateTime.Now;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Entry(q).State = EntityState.Modified;
            _dbContext.Entry(fitbq).State = EntityState.Modified;

            var result = await _dbContext.SaveChangesAsync() > 0;
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateConstructedResponseQuestion(Question q, ConstructedResponseQuestion crq)
    {
        var existingQuestion = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == q.Id);
        if (existingQuestion == null) return false;

        q.CreatedAt = existingQuestion.CreatedAt;
        q.UpdatedAt = DateTime.Now;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Entry(q).State = EntityState.Modified;
            _dbContext.Entry(crq).State = EntityState.Modified;

            var result = await _dbContext.SaveChangesAsync() > 0;
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteQuestion(Question q)
    {
        _dbContext.Questions.Remove(q);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
