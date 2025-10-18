using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

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

    [HttpPost]
    public async Task<IActionResult> AddExam([FromBody] ExamAttemptDTO examAttemptDTO)
    {
        ExamAttempt examAttempt = new ExamAttempt
        {
            ExamId = examAttemptDTO.ExamId,
            StudentId = examAttemptDTO.StudentId,
            Score = examAttemptDTO.Score,
            Duration = examAttemptDTO.Duration,
            SubmittedAt = DateTime.Now
        };

        bool added = await _examAttemptDb.AddExamAttempt(examAttempt);
        if (!added) return StatusCode(400, "Failed to add exam attempt.");
        var newId = await _examAttemptDb.GetLastIdAsync();

        try
        {
            await using var transaction = await _examAttemptDb.DbContext.Database.BeginTransactionAsync();
            foreach (var examAttemptAnswerDTO in examAttemptDTO.ExamAttemptAnswers)
            {
                var answer = new ExamAttemptAnswer
                {
                    ExamAttemptId = newId,
                    ExamQuestionId = examAttemptAnswerDTO.ExamQuestionId,
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
            return StatusCode(500);
        }
    }
}