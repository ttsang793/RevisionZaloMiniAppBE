using backend.DTOs;
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

    public async Task<ExamAttemptGetDTO> GetLatestExamAttempt(ulong studentId, ulong examId)
    {
        var examAttempt = await DbContext.ExamAttempts
            .Where(ea => ea.StudentId == studentId && ea.ExamId == examId)
            .Include(ea => ea.ExamAttemptAnswers)
            .ThenInclude(eaa => eaa.ExamQuestion)
            .ThenInclude(eq => eq.Question)
            .FirstAsync();

        var parts = DbContext.ExamParts
            .Where(ep => examAttempt.PartOrder.Contains(ep.Id))
            .Select(ep => new ExamPart { Id = ep.Id, PartTitle = ep.PartTitle })            
            .ToList();

        var orderedParts = examAttempt.PartOrder
            .Select(id => parts.First(p => p.Id == id))
            .ToList();

        var questionIds = examAttempt.ExamAttemptAnswers
            .Select(ea => ea.ExamQuestion.Question.Id)
            .Distinct().ToList();

        var mcQuestions = await DbContext.MultipleChoiceQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var tfQuestions = await DbContext.TrueFalseQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var saQuestions = await DbContext.ShortAnswerQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var mrQuestions = await DbContext.ManualResponseQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var sortingQuestions = await DbContext.SortingQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var tfTHPTQuestions = await DbContext.TrueFalseTHPTQuestions
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id);

        var examParts = new List<ExamAttemptPartDTO>();

        foreach (var part in orderedParts)
        {
            List<ExamQuestionDTO> examQuestionsDTO = [];
            List<ExamAttemptAnswerGetDTO> examAttemptAnswers = [];

            foreach (var eaa in examAttempt.ExamAttemptAnswers)
            {
                if (eaa.ExamQuestion.ExamPartId != part.Id) continue;
                ExamQuestion eq = eaa.ExamQuestion;
                QuestionDTO questionDTO;

                switch (eq.Question.Type.ToLower())
                {
                    case "multiple-choice":
                        if (mcQuestions.TryGetValue(eq.Question.Id, out var mcq))
                            questionDTO = new MultipleChoiceQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                Type = eq.Question.Type,
                                Explanation = eq.Question.Explanation,
                                CorrectAnswer = mcq.CorrectAnswer
                            };
                        else throw new Exception();
                        break;

                    case "true-false":
                        if (tfQuestions.TryGetValue(eq.Question.Id, out var tfq))
                            questionDTO = new TrueFalseQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                Type = eq.Question.Type,
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
                                Type = eq.Question.Type,
                                Explanation = eq.Question.Explanation,
                                AnswerKey = saq.AnswerKey
                            };
                        else continue;
                        break;

                    case "fill-in-the-blank":
                    case "constructed-response":
                        if (mrQuestions.TryGetValue(eq.Question.Id, out var mrq))
                            questionDTO = new ManualResponseQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                Type = eq.Question.Type,
                                Explanation = eq.Question.Explanation,
                                AnswerKeys = mrq.AnswerKeys,
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
                                Type = eq.Question.Type,
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
                                Type = eq.Question.Type,
                                Explanation = eq.Question.Explanation,
                                Statements = tfTHPT.Statements,
                                AnswerKeys = tfTHPT.AnswerKeys
                            };
                        else continue;
                        break;

                    default: continue;
                }

                examAttemptAnswers.Add(new ExamAttemptAnswerGetDTO
                {
                    Point = eq.Point,
                    Question = questionDTO,
                    AnswerOrder = eaa.AnswerOrder,
                    StudentAnswer = eaa.StudentAnswer,
                    Correct = eaa.Correct
                });
            }

            ExamAttemptPartDTO partDTO = new ExamAttemptPartDTO
            {
                PartTitle = part.PartTitle,
                ExamAttemptAnswers = examAttemptAnswers
            };

            examParts.Add(partDTO);
        }

        ExamAttemptGetDTO result = new ExamAttemptGetDTO
        {
            TotalPoint = (decimal)examAttempt.TotalPoint,
            Duration = examAttempt.Duration,
            Comment = examAttempt.Comment,
            ExamParts = examParts
        };

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

    public async Task<PdfExamAttempt> GetLatestPdfExamAttempt(ulong studentId, ulong examId)
    {
        var result = await (from pea in DbContext.PdfExamAttempts
                            join pe in DbContext.PdfExamCodes
                            on pea.PdfExamCodeId equals pe.Id
                            where pea.StudentId == studentId && pe.ExamId == examId
                            orderby pea.SubmittedAt descending
                            select pea).FirstAsync();
        return result;
    }

    public async Task<bool> AddPdfExamAttempt(PdfExamAttempt pdfExamAttempt)
    {
        DbContext.PdfExamAttempts.Add(pdfExamAttempt);
        return await DbContext.SaveChangesAsync() > 0;
    }
}
