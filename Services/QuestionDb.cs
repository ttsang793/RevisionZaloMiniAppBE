using backend.Models;
using backend.DTOs;
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
    public async Task<List<QuestionReadDTO>> GetAllQuestionsByTeacher(ulong teacherId, string? title)
    {
        return await (from q in _dbContext.Questions
                      join t in _dbContext.Teachers on q.TeacherId equals t.Id
                      join s in _dbContext.Subjects on t.SubjectId equals s.Id
                      where t.Id == teacherId && (string.IsNullOrEmpty(title) || q.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                      select new QuestionReadDTO
                      {
                          Id = q.Id,
                          Title = q.Title,
                          Grade = q.Grade,
                          Type = q.Type,
                          Difficulty = q.Difficulty,
                          TopicId = q.TopicId,
                          TeacherId = q.TeacherId,
                          SubjectName = s.Name,
                          Explanation = q.Explanation
                      }).ToListAsync();
    }

    public async Task<List<QuestionReadDTO>> GetFilterQuestionsByTeacher(ulong teacherId, string type, string? title)
    {
        return await (from q in _dbContext.Questions
                      join t in _dbContext.Topics on q.TopicId equals t.Id
                      join s in _dbContext.Subjects on t.SubjectId equals s.Id
                      where q.Type == type && (string.IsNullOrEmpty(title) || q.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                      select new QuestionReadDTO
                      {
                          Id = q.Id,
                          Title = q.Title,
                          Grade = q.Grade,
                          Type = q.Type,
                          Difficulty = q.Difficulty,
                          TopicId = q.TopicId,
                          SubjectName = s.Name,
                          Explanation = q.Explanation
                      }).ToListAsync();
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

    /*
    public async Task<GroupQuestion> GetGroupQuestionById(ulong id)
    {
        return await _dbContext.GroupQuestions.FirstAsync(q => q.Id == id);
    }
    */

    public async Task<TrueFalseTHPTQuestion> GetTrueFalseTHPTQuestionById(ulong id)
    {
        return await _dbContext.TrueFalseTHPTQuestions.FirstAsync(q => q.Id == id);
    }

    // POST
    public async Task<bool> AddMultipleChoiceQuestion(Question q, MultipleChoiceQuestion mcq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            mcq.Id = q.Id;
            _dbContext.MultipleChoiceQuestions.Add(mcq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddTrueFalseQuestion(Question q, TrueFalseQuestion tfq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            tfq.Id = q.Id;
            _dbContext.TrueFalseQuestions.Add(tfq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddShortAnswerQuestion(Question q, ShortAnswerQuestion saq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            saq.Id = q.Id;
            _dbContext.ShortAnswerQuestions.Add(saq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddManualResponseQuestion(Question q, ManualResponseQuestion mrq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            mrq.Id = q.Id;
            _dbContext.ManualResponseQuestions.Add(mrq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddSortingQuestion(Question q, SortingQuestion sq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            sq.Id = q.Id;
            _dbContext.SortingQuestions.Add(sq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    /*
    public async Task<bool> AddGroupQuestion(Question q, GroupQuestion gq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            gq.Id = q.Id;
            _dbContext.GroupQuestions.Add(gq);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
    */

    public async Task<bool> AddTrueFalseTHPTQuestion(Question q, TrueFalseTHPTQuestion tfq)
    {
        _dbContext.Questions.Add(q);

        if (await _dbContext.SaveChangesAsync() > 0)
        {
            tfq.Id = q.Id;
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
