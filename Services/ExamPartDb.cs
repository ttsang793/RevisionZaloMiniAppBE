using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamPartDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public ExamPartDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExamPart>> GetExamPartsAsyncByExamId(ulong id)
    {
        var result = await (from ep in _dbContext.ExamParts
                            where ep.ExamId == id
                            select new ExamPart
                            {
                                Id = ep.Id,
                                PartTitle = ep.PartTitle,
                                QuestionTypes = ep.QuestionTypes,
                                ExamQuestions = _dbContext.ExamQuestions.Where(eq => eq.ExamPartId == ep.Id).ToList()
                            }).ToListAsync();

        return result;
    }

    public async Task<List<ExamPartDTO>> GetQuestionListForAttemptAsync(ulong id)
    {
        var examParts = await _dbContext.ExamParts
            .Where(ep => ep.ExamId == id)
            .Include(ep => ep.ExamQuestions)
            .ThenInclude(eq => eq.Question)
            .ToListAsync();

        var questionIds = examParts
            .SelectMany(ep => ep.ExamQuestions.Select(eq => eq.Question.Id))
            .Distinct().ToList();

        var mcQuestions = await _dbContext.MultipleChoiceQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var tfQuestions = await _dbContext.TrueFalseQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var saQuestions = await _dbContext.ShortAnswerQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var mrQuestions = await _dbContext.ManualResponseQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var sortingQuestions = await _dbContext.SortingQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var tfTHPTQuestions = await _dbContext.TrueFalseTHPTQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var result = new List<ExamPartDTO>();

        foreach (ExamPart ep in examParts)
        {
            List<ExamQuestionDTO> examQuestionsDTO = [];

            foreach (var eq in ep.ExamQuestions)
            {
                QuestionDTO questionDTO;

                switch (eq.Question.Type.ToLower())
                {
                    case "multiple-choice":
                        if (mcQuestions.TryGetValue(eq.Question.Id, out var mcq))
                            questionDTO = new MultipleChoiceQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Difficulty = eq.Question.Difficulty,
                                TopicId = eq.Question.TopicId,
                                Explanation = eq.Question.Explanation,
                                CorrectAnswer = mcq.CorrectAnswer,
                                WrongAnswer = mcq.WrongAnswer,
                            };
                        else throw new Exception();
                        break;

                    case "true-false":
                        if (tfQuestions.TryGetValue(eq.Question.Id, out var tfq))
                            questionDTO = new TrueFalseQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Difficulty = eq.Question.Difficulty,
                                TopicId = eq.Question.TopicId,
                                Explanation = eq.Question.Explanation,
                                AnswerKey = tfq.AnswerKey
                            };
                        else continue;
                        break;

                    case "short-answer":
                        if (saQuestions.TryGetValue(eq.Question.Id, out var saq))
                            questionDTO = new ShortAnswerQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Difficulty = eq.Question.Difficulty,
                                TopicId = eq.Question.TopicId,
                                Explanation = eq.Question.Explanation,
                                AnswerKey = saq.AnswerKey
                            };
                        else continue;
                        break;

                    case "gap-fill": case "constructed-response":
                        if (mrQuestions.TryGetValue(eq.Question.Id, out var mrq))
                            questionDTO = new ManualResponseQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Difficulty = eq.Question.Difficulty,
                                TopicId = eq.Question.TopicId,
                                Explanation = eq.Question.Explanation,
                                AnswerKeys = mrq.AnswerKeys,
                                AllowTakePhoto = mrq.AllowTakePhoto,
                                AllowEnter = mrq.AllowEnter,
                                MarkAsWrong = mrq.MarkAsWrong
                            };
                        else continue;
                        break;

                    case "sorting":
                        if (sortingQuestions.TryGetValue(eq.Question.Id, out var sorting))
                            questionDTO = new SortingQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Difficulty = eq.Question.Difficulty,
                                TopicId = eq.Question.TopicId,
                                Explanation = eq.Question.Explanation,
                                CorrectOrder = sorting.CorrectOrder
                            };
                        else continue;
                        break;

                    case "true-false-thpt":
                        if (tfTHPTQuestions.TryGetValue(eq.Question.Id, out var tfTHPT))
                            questionDTO = new TrueFalseTHPTQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Difficulty = eq.Question.Difficulty,
                                TopicId = eq.Question.TopicId,
                                Explanation = eq.Question.Explanation,
                                Statements = tfTHPT.Statements,
                                AnswerKeys = tfTHPT.AnswerKeys
                            };
                        else continue;
                        break;

                    default: continue;
                }

                examQuestionsDTO.Add(new ExamQuestionDTO
                {
                    Id = eq.Id,
                    OrderIndex = eq.OrderIndex,
                    Point = eq.Point,
                    Question = questionDTO
                });
            }

            result.Add(new ExamPartDTO
            {
                Id = ep.Id,
                PartTitle = ep.PartTitle,
                ExamQuestions = examQuestionsDTO
            });
        }

        return result;
    }

    public async Task<ExamPart> GetExamPartById(ulong id)
    {
        return await _dbContext.ExamParts.FirstOrDefaultAsync(e => e.ExamId == id);
    }

    public async Task<bool> AddExamPart(ExamPart ep)
    {
        _dbContext.ExamParts.Add(ep);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExamPart(ulong id)
    {
        _dbContext.ExamParts.Remove(await GetExamPartById(id));

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExamPartsByExamId(ulong examId)
    {
        var examParts = _dbContext.ExamParts.Where(ep => ep.ExamId == examId).ToList();
        _dbContext.ExamParts.RemoveRange(examParts);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
