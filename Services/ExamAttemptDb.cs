using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace backend.Services;

public class ExamAttemptDb
{
    public ZaloRevisionAppDbContext DbContext { get; }

    public ExamAttemptDb(ZaloRevisionAppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<List<ExamAttemptGetDTO>> GetExamAttemptsAsync(ulong examId)
    {
        var result = await (from ea in DbContext.ExamAttempts
                            join e in DbContext.Exams on ea.ExamId equals e.Id
                            where e.Id == examId
                            orderby ea.Id descending
                            select new ExamAttemptGetDTO
                            {
                                Id = ea.Id,
                                ExamTitle = e.Title,
                                TotalPoint = ea.TotalPoint.HasValue ? ea.TotalPoint.Value : 0,
                                Duration = ea.Duration,
                                SubmittedAt = ea.SubmittedAt,
                                MarkedAt = ea.MarkedAt
                            }).ToListAsync();

        return result;
    }

    public async Task<ExamAttemptStatDTO> GetExamRecordByExamId(ulong examId)
    {
        var result = await (from ea in DbContext.ExamAttempts
                            where ea.ExamId == examId
                            orderby ea.TotalPoint descending, ea.Duration ascending
                            select new ExamAttemptStatDTO
                            {
                                MaxTotalPoint = ea.TotalPoint,
                                Duration = ea.Duration
                            }).FirstOrDefaultAsync();

        if (result == null) result = new ExamAttemptStatDTO();
        result.Count = await DbContext.ExamAttempts.Where(ea => ea.ExamId == examId).CountAsync();
        return result;
    }

    public async Task<ExamAttemptGetDTO> GetExamAttempt(ulong? studentId, ulong? examId, ulong? examAttemptId)
    {
        ExamAttempt examAttempt;

        if (examAttemptId.HasValue)
        {
            examAttempt = await DbContext.ExamAttempts
                    .Include(ea => ea.ExamAttemptAnswers)
                    .ThenInclude(eaa => eaa.ExamQuestion)
                    .ThenInclude(eq => eq.Question)
                    .Where(eaa => eaa.Id == examAttemptId)
                    .FirstAsync();
        }

        else examAttempt = await DbContext.ExamAttempts
                    .Include(ea => ea.ExamAttemptAnswers)
                    .ThenInclude(eaa => eaa.ExamQuestion)
                    .ThenInclude(eq => eq.Question)
                    .Where(ea => ea.StudentId == studentId && ea.ExamId == examId)
                    .OrderByDescending(ea => ea.Id)
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
                                ImageUrl = eq.Question.ImageUrl,
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
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Explanation = eq.Question.Explanation,
                                AnswerKey = tfq.AnswerKey
                            };
                        else continue;
                        break;

                    case "short-answer":
                        if (saQuestions.TryGetValue(eq.Question.Id, out var saq))
                        {
                            while (saq.AnswerKey.Length < 4) saq.AnswerKey += " ";

                            questionDTO = new ShortAnswerQuestionDTO
                            {
                                Id = eq.QuestionId,
                                Title = eq.Question.Title,
                                ImageUrl = eq.Question.ImageUrl,
                                Type = eq.Question.Type,
                                Explanation = eq.Question.Explanation,
                                AnswerKey = saq.AnswerKey
                            };
                        }
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
                                ImageUrl = eq.Question.ImageUrl,
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
                                ImageUrl = eq.Question.ImageUrl,
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
                    Id = eaa.Id,
                    CorrectPoint = eq.Point,
                    Point = eaa.Point.Value,
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
            StudentId = examAttempt.StudentId,
            TotalPoint = examAttempt.TotalPoint.HasValue ? examAttempt.TotalPoint.Value : 0,
            Duration = examAttempt.Duration,
            Comment = examAttempt.Comment,
            ExamParts = examParts
        };

        return result;
    }

    public async Task<DateTime?> GetLatestExamAttemptDateAsync(ulong studentId, ulong examId)
    {
        try
        {
            var exam = await DbContext.ExamAttempts
                        .Include(ea => ea.ExamAttemptAnswers)
                        .ThenInclude(eaa => eaa.ExamQuestion)
                        .ThenInclude(eq => eq.Question)
                        .Where(ea => ea.StudentId == studentId && ea.ExamId == examId)
                        .OrderByDescending(ea => ea.Id)
                        .FirstAsync();

            return (exam == null) ? null : exam.SubmittedAt;
        }
        catch
        {
            return null;
        }
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

    public async Task<bool> GradingExamAttempt(ExamAttempt examAttempt)
    {
        var existingAttempt = DbContext.ExamAttempts.First(ea => ea.Id == examAttempt.Id);
        existingAttempt.Comment = examAttempt.Comment;
        existingAttempt.TotalPoint = examAttempt.TotalPoint;
        existingAttempt.MarkedAt = DateTime.Now;
        DbContext.ExamAttempts.Update(existingAttempt);

        foreach (var examAttemptAnswer in examAttempt.ExamAttemptAnswers)
        {
            var existingAnswer = DbContext.ExamAttemptAnswers.First(eaa => eaa.Id == examAttemptAnswer.Id);
            existingAnswer.Correct = examAttemptAnswer.Correct;
            existingAnswer.Point = examAttemptAnswer.Point;
            DbContext.ExamAttemptAnswers.Update(existingAnswer);
        }

        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveLastIndex()
    {
        DbContext.ExamAttempts.Remove((await DbContext.ExamAttempts.OrderByDescending(e => e.Id).ToListAsync())[0]);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<PdfExamAttemptReadDTO> GetLatestPdfExamAttempt(ulong studentId, ulong examId)
    {
        var result = await (from pea in DbContext.PdfExamAttempts
                            join pe in DbContext.PdfExamCodes on pea.PdfExamCodeId equals pe.Id
                            join ea in DbContext.ExamAttempts on pea.Id equals ea.Id
                            where ea.StudentId == studentId && pe.ExamId == examId
                            orderby ea.SubmittedAt descending
                            select new PdfExamAttemptReadDTO
                            {
                                Id = pea.Id,
                                ExamId = ea.ExamId,
                                TaskPdf = pe.TaskPdf!,
                                AnswerPdf = pe.AnswerPdf!,
                                TotalPoint = ea.TotalPoint.Value,
                                Comment = ea.Comment,
                                PdfExamCodeId = pe.Id,
                                StudentAnswer = pea.StudentAnswer,
                                CorrectBoard = pea.CorrectBoard,
                                PointBoard = pea.PointBoard
                            }).FirstAsync();
        return result;
    }

    public async Task<PdfExamAttemptReadDTO> GetPdfExamAttemptById(ulong examAttemptId)
    {
        var result = await (from pea in DbContext.PdfExamAttempts
                            join pe in DbContext.PdfExamCodes on pea.PdfExamCodeId equals pe.Id
                            join ea in DbContext.ExamAttempts on pea.Id equals ea.Id
                            where pea.Id == examAttemptId
                            select new PdfExamAttemptReadDTO
                            {
                                Id = pea.Id,
                                ExamId = ea.ExamId,
                                TaskPdf = pe.TaskPdf!,
                                AnswerPdf = pe.AnswerPdf!,
                                TotalPoint = ea.TotalPoint.Value,
                                Comment = ea.Comment,
                                PdfExamCodeId = pe.Id,
                                StudentAnswer = pea.StudentAnswer,
                                CorrectBoard = pea.CorrectBoard,
                                PointBoard = pea.PointBoard
                            }).FirstAsync();
        return result;
    }

    public async Task<bool> AddPdfExamAttempt(PdfExamAttempt pdfExamAttempt)
    {
        DbContext.PdfExamAttempts.Add(pdfExamAttempt);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> GradingPdfExamAttempt(ExamAttempt examAttempt, PdfExamAttempt pdfExamAttempt)
    {
        var existingAttempt = await DbContext.ExamAttempts.FirstOrDefaultAsync(ea => ea.Id == examAttempt.Id);
        var existingPdfAttempt = await DbContext.PdfExamAttempts.FirstOrDefaultAsync(pea => pea.Id == pdfExamAttempt.Id);

        if (existingAttempt == null || existingPdfAttempt == null) return false;

        existingAttempt.Comment = examAttempt.Comment;
        existingAttempt.TotalPoint = examAttempt.TotalPoint;
        existingAttempt.MarkedAt = DateTime.Now;
        DbContext.ExamAttempts.Update(existingAttempt);

        existingPdfAttempt.PointBoard = pdfExamAttempt.PointBoard;
        existingPdfAttempt.CorrectBoard = pdfExamAttempt.CorrectBoard;

        DbContext.PdfExamAttempts.Update(existingPdfAttempt);

        return await DbContext.SaveChangesAsync() > 0;
    }
}
