using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/exam-attempt")]
public class ExamAttemptController : Controller
{
    private readonly ILogger<QuestionController> _logger;
    private readonly ExamAttemptDb _examAttemptDb;

    public ExamAttemptController(ILogger<QuestionController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _examAttemptDb = new ExamAttemptDb(dbContext);
    }

    [HttpGet]
    public async Task<ExamAttemptGetDTO> GetLatestExamAttempt(ulong studentId, ulong examId)
    {
        return await _examAttemptDb.GetLatestExamAttempt(studentId, examId);
    }

    [HttpGet("pdf")]
    public async Task<PdfExamAttempt> GetLatestPdfExamAttempt(ulong studentId, ulong examId)
    {
        return await _examAttemptDb.GetLatestPdfExamAttempt(studentId, examId);
    }

    [HttpPost]
    public async Task<IActionResult> AddExamAttempt([FromBody] ExamAttemptDTO examAttemptDTO)
    {
        double durationDouble = (DateTime.Now - examAttemptDTO.StartedAt.ToLocalTime()).TotalSeconds;
        ushort duration = ((ushort)Math.Round(durationDouble));

        ExamAttempt examAttempt = new ExamAttempt
        {
            ExamId = examAttemptDTO.ExamId,
            StudentId = examAttemptDTO.StudentId,
            Score = examAttemptDTO.Score,
            StartedAt = examAttemptDTO.StartedAt.ToLocalTime(),
            IsPractice = examAttemptDTO.IsPractice,
            Duration = duration,
            SubmittedAt = DateTime.Now,
            PartOrder = examAttemptDTO.PartOrder
        };

        var addedExam = await _examAttemptDb.AddExamAttempt(examAttempt);
        if (addedExam == null) return StatusCode(400, "Failed to add exam attempt!");
        var newId = addedExam.Id;
        try
        {
            await using var transaction = await _examAttemptDb.DbContext.Database.BeginTransactionAsync();
            foreach (var examAttemptAnswerDTO in examAttemptDTO.ExamAttemptAnswers)
            {
                var answer = new ExamAttemptAnswer
                {
                    ExamAttemptId = newId,
                    ExamQuestionId = examAttemptAnswerDTO.ExamQuestionId,
                    AnswerOrder = examAttemptAnswerDTO.AnswerOrder,
                    StudentAnswer = examAttemptAnswerDTO.StudentAnswer,
                    AnswerType = examAttemptAnswerDTO.AnswerType,
                    IsCorrect = examAttemptAnswerDTO.IsCorrect
                };

                bool answerAdded = await _examAttemptDb.AddExamAttemptAnswer(answer);
                if (!answerAdded)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(400, "Failed to add one of the exam answers.");
                }
            }

            await transaction.CommitAsync();
            return StatusCode(201, "Submit successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error submiting the exam on {DateTime.Now}");
            await _examAttemptDb.RemoveLastIndex();
            return StatusCode(500);
        }
    }

    [HttpPost("pdf")]
    public async Task<IActionResult> AddPdfExamAttempt(PdfExamAttemptDTO pdfExamAttemptDTO)
    {
        double durationDouble = (DateTime.Now - pdfExamAttemptDTO.StartedAt.ToLocalTime()).TotalSeconds;
        ushort duration = ((ushort)Math.Round(durationDouble));

        var pdfExamAttempt = new PdfExamAttempt
        {
            ExamId = pdfExamAttemptDTO.ExamId,
            StudentId = pdfExamAttemptDTO.StudentId,
            Score = pdfExamAttemptDTO.Score,
            StartedAt = pdfExamAttemptDTO.StartedAt.ToLocalTime(),
            Duration = duration,
            SubmittedAt = DateTime.Now,
            StudentAnswer = pdfExamAttemptDTO.StudentAnswer,
            ScoreBoard = pdfExamAttemptDTO.ScoreBoard,
            CodeId = pdfExamAttemptDTO.CodeId
        };

        return await _examAttemptDb.AddPdfExamAttempt(pdfExamAttempt) ? StatusCode(201) : StatusCode(400);
    }
}