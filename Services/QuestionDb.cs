using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;

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
                          ImageUrl = q.ImageUrl,
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
                          ImageUrl = q.ImageUrl,
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
    public async Task<bool> CheckIfExports(ulong id)
    {
        try
        {
            var status = await (from eq in _dbContext.ExamQuestions
                                join ep in _dbContext.ExamParts on eq.ExamPartId equals ep.Id
                                join e in _dbContext.Exams on ep.ExamId equals e.Id
                                where eq.QuestionId == id
                                select e.Status).FirstAsync();

            return (status == 3);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateQuestion(Question q, ulong id)
    {
        var question = await _dbContext.Questions.Where(q => q.Id == id).FirstAsync();
        if (question == null) return false;

        question.Title = q.Title;
        question.Grade = q.Grade;
        question.Type = q.Type;
        question.Difficulty = q.Difficulty;
        question.TopicId = q.TopicId;
        question.Explanation = q.Explanation;

        _dbContext.Questions.Update(question);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateMultipleChoiceQuestion(Question q, MultipleChoiceQuestion mcq, ulong id)
    {
        var multipleChoice = await _dbContext.MultipleChoiceQuestions.Where(mcq => mcq.Id == id).FirstAsync();

        if (multipleChoice == null || await UpdateQuestion(q, id) == false) return false;
        multipleChoice.CorrectAnswer = mcq.CorrectAnswer;
        multipleChoice.WrongAnswer = mcq.WrongAnswer;
        _dbContext.MultipleChoiceQuestions.Update(multipleChoice);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTrueFalseQuestion(Question q, TrueFalseQuestion tfq, ulong id)
    {
        var trueFalse = await _dbContext.TrueFalseQuestions.Where(tfq => tfq.Id == id).FirstAsync();

        if (trueFalse == null || await UpdateQuestion(q, id) == false) return false;
        trueFalse.AnswerKey = tfq.AnswerKey;
        _dbContext.TrueFalseQuestions.Update(trueFalse);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateShortAnswerQuestion(Question q, ShortAnswerQuestion saq, ulong id)
    {
        var shortAnswer = await _dbContext.ShortAnswerQuestions.Where(saq => saq.Id == id).FirstAsync();

        if (shortAnswer == null || await UpdateQuestion(q, id) == false) return false;
        shortAnswer.AnswerKey = saq.AnswerKey;
        _dbContext.ShortAnswerQuestions.Update(shortAnswer);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateManualResponseQuestion(Question q, ManualResponseQuestion mrq, ulong id)
    {
        var manualResponse = await _dbContext.ManualResponseQuestions.Where(mrq => mrq.Id == id).FirstAsync();

        if (manualResponse == null || await UpdateQuestion(q, id) == false) return false;
        manualResponse.AnswerKeys = mrq.AnswerKeys;
        manualResponse.AllowTakePhoto = mrq.AllowTakePhoto;
        manualResponse.AllowEnter = mrq.AllowEnter;
        manualResponse.MarkAsWrong = mrq.MarkAsWrong;
        _dbContext.ManualResponseQuestions.Update(manualResponse);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateSortingQuestion(Question q, SortingQuestion sq, ulong id)
    {
        var sorting = await _dbContext.SortingQuestions.Where(sq => sq.Id == id).FirstAsync();

        if (sorting == null || await UpdateQuestion(q, id) == false) return false;
        sorting.CorrectOrder = sq.CorrectOrder;
        _dbContext.SortingQuestions.Update(sorting);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateTrueFalseTHPTQuestion(Question q, TrueFalseTHPTQuestion tfq, ulong id)
    {
        var trueFalseTHPT = await _dbContext.TrueFalseTHPTQuestions.Where(tfq => tfq.Id == id).FirstAsync();

        if (trueFalseTHPT == null || await UpdateQuestion(q, id) == false) return false;
        trueFalseTHPT.PassageTitle = tfq.PassageTitle;
        trueFalseTHPT.PassageContent = tfq.PassageContent;
        trueFalseTHPT.PassageAuthor = tfq.PassageAuthor;
        trueFalseTHPT.Statements = tfq.Statements;
        trueFalseTHPT.AnswerKeys = tfq.AnswerKeys;
        _dbContext.TrueFalseTHPTQuestions.Update(trueFalseTHPT);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateQuestionImage(ulong id, string imageUrl)
    {
        var question = await GetQuestionById(id);
        question.ImageUrl = imageUrl;
        _dbContext.Update(question);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteQuestion(Question q)
    {
        _dbContext.Questions.Remove(q);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
