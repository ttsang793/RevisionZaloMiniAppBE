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

    // GET LIST<QUESTION>
    public async Task<List<Question>> GetAllQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%")).ToListAsync();
        return await _dbContext.Questions.ToListAsync();
    }

    public async Task<List<Question>> GetMultipleChoiceQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "multiple-choice").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "multiple-choice").ToListAsync();
    }

    public async Task<List<Question>> GetTrueFalseQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "true-false").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "true-false").ToListAsync();
    }

    public async Task<List<Question>> GetShortAnswerQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "short-answer").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "short-answer").ToListAsync();
    }

    public async Task<List<Question>> GetFillInTheBlankQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "fill-in-the-blank").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "fill-in-the-blank").ToListAsync();
    }

    public async Task<List<Question>> GetConstructedResponseQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "constructed-response").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "constructed-response").ToListAsync();
    }

    public async Task<List<Question>> GetSortingQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "sorting").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "sorting").ToListAsync();
    }

    public async Task<List<Question>> GetDefaultQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type != "true-false-thpt" && q.Type != "group").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type != "true-false-thpt" && q.Type != "group").ToListAsync();
    }

    public async Task<List<Question>> GetGroupQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "group").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "group").ToListAsync();
    }

    public async Task<List<Question>> GetTrueFalseTHPTQuestionsByTeacher(string? title)
    {
        if (!string.IsNullOrEmpty(title))
            return await _dbContext.Questions.Where(q => EF.Functions.Like(q.Title, $"%{title}%") && q.Type == "true-false-thpt").ToListAsync();
        return await _dbContext.Questions.Where(q => q.Type == "true-false-thpt").ToListAsync();
    }

    // GET BY ID
    public async Task<Question> GetQuestionById(ulong id)
    {
        return await _dbContext.Questions.FirstAsync(q => q.Id == id);
    }

    public async Task<List<Question>> GetQuestionByMultipleIds(ICollection<ulong> id)
    {
        return await _dbContext.Questions.Where(q => id.Contains(q.Id)).ToListAsync();
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

    public async Task<ManualResponseQuestion> GetManualResponseQuestionById(ulong id)
    {
        return await _dbContext.ManualResponseQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<SortingQuestion> GetSortingQuestionById(ulong id)
    {
        return await _dbContext.SortingQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<GroupQuestion> GetGroupQuestionById(ulong id)
    {
        return await _dbContext.GroupQuestions.FirstAsync(q => q.Id == id);
    }

    public async Task<TrueFalseTHPTQuestion> GetTrueFalseTHPTQuestionById(ulong id)
    {
        return await _dbContext.TrueFalseTHPTQuestions.FirstAsync(q => q.Id == id);
    }

    private async Task<ulong> GetLastIndex()
    {
        return (await _dbContext.Questions.OrderBy(s => s.Id).LastAsync()).Id;
    }

    // POST
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

    public async Task<bool> AddManualResponseQuestion(Question q, ManualResponseQuestion mrq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            mrq.Id = await GetLastIndex();
            _dbContext.ManualResponseQuestions.Add(mrq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddSortingQuestion(Question q, SortingQuestion sq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            sq.Id = await GetLastIndex();
            _dbContext.SortingQuestions.Add(sq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddGroupQuestion(Question q, GroupQuestion gq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            gq.Id = await GetLastIndex();
            _dbContext.GroupQuestions.Add(gq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddTrueFalseTHPTQuestion(Question q, TrueFalseTHPTQuestion tfq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            tfq.Id = await GetLastIndex();
            _dbContext.TrueFalseTHPTQuestions.Add(tfq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    // PUT AND DELETE
    public async Task<bool> UpdateQuestion(Question q, TypeQuestion tq)
    {
        var existingQuestion = await _dbContext.Questions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == q.Id);
        if (existingQuestion == null) return false;

        q.CreatedAt = existingQuestion.CreatedAt;

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Entry(q).State = EntityState.Modified;
            _dbContext.Entry(tq).State = EntityState.Modified;

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
