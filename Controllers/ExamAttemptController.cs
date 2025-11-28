using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/exam-attempt")]
public class ExamAttemptController : Controller
{
    private readonly ILogger<QuestionController> _logger;
    private readonly ExamAttemptDb _examAttemptDb;
    private readonly AchievementDb _achievementDb;

    public ExamAttemptController(ILogger<QuestionController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _examAttemptDb = new ExamAttemptDb(dbContext);
        _achievementDb = new AchievementDb(dbContext);
    }

    [HttpGet("{studentId}/{examId}")]
    public async Task<ExamAttemptGetDTO> GetLatestExamAttempt(ulong studentId, ulong examId)
    {
        return await _examAttemptDb.GetLatestExamAttempt(studentId, examId);
    }

    [HttpGet("pdf/{studentId}/{examId}")]
    public async Task<PdfExamAttempt> GetLatestPdfExamAttempt(ulong studentId, ulong examId)
    {
        return await _examAttemptDb.GetLatestPdfExamAttempt(studentId, examId);
    }
    
    #pragma warning disable CS8629 // Nullable value type may be null.
    [HttpPost]
    public async Task<IActionResult> AddExamAttempt([FromBody] ExamAttemptDTO examAttemptDTO)
    {

        double durationDouble = (DateTime.Now - ((DateTime)examAttemptDTO.StartedAt).ToLocalTime()).TotalSeconds;
        ushort duration = ((ushort)Math.Round(durationDouble));

        ExamAttempt examAttempt = new ExamAttempt
        {
            ExamId = (ulong)examAttemptDTO.ExamId,
            StudentId = (ulong)examAttemptDTO.StudentId,
            TotalPoint = examAttemptDTO.TotalPoint,
            StartedAt = ((DateTime)examAttemptDTO.StartedAt).ToLocalTime(),
            Duration = duration,
            SubmittedAt = DateTime.Now,
            PartOrder = examAttemptDTO.PartOrder
        };
        
        var addedExam = await _examAttemptDb.AddExamAttempt(examAttempt);
        if (addedExam == null) return StatusCode(400, "Nộp bài thất bại!");
        var newId = addedExam.Id;
        try
        {
            await using var transaction = await _examAttemptDb.DbContext.Database.BeginTransactionAsync();
            foreach (var examAttemptAnswerDTO in examAttemptDTO.ExamAttemptAnswers)
            {
                var answer = new ExamAttemptAnswer
                {
                    ExamAttemptId = newId,
                    ExamQuestionId = (ulong)examAttemptAnswerDTO.ExamQuestionId,
                    AnswerOrder = examAttemptAnswerDTO.AnswerOrder,
                    StudentAnswer = examAttemptAnswerDTO.StudentAnswer,
                    Correct = examAttemptAnswerDTO.Correct,
                    Point = examAttemptAnswerDTO.Point
                };

                bool answerAdded = await _examAttemptDb.AddExamAttemptAnswer(answer);
                if (!answerAdded)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(400, "Nộp bài thất bại do thêm đáp án thất bại!");
                }
            }

            await transaction.CommitAsync();
            return StatusCode(201, "Nộp bài thành công!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Nộp bài thất bại vào {DateTime.Now}");
            await _examAttemptDb.RemoveLastIndex();
            return StatusCode(500);
        }
    }
    #pragma warning restore CS8629 // Nullable value type may be null.

    [HttpPut("comment/{id}")]
    public async Task<IActionResult> CommentExamAttempt([FromBody] ExamAttemptDTO examAttemptDTO, ulong id)
    {
        ExamAttempt examAttempt = new ExamAttempt
        {
            Id = id,
            Comment = examAttemptDTO.Comment
        };

        return (await _examAttemptDb.CommentExamAttempt(examAttempt)) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("grading/{id}")]
    public async Task<IActionResult> GradingExamAttempt([FromBody] ExamAttemptDTO examAttemptDTO, ulong id)
    {
        ExamAttempt examAttempt = new ExamAttempt
        {
            Id = id,
            TotalPoint = examAttemptDTO.TotalPoint
        };

        foreach (var examAttemptAnswerDTO in examAttemptDTO.ExamAttemptAnswers)
            examAttempt.ExamAttemptAnswers.Add(new ExamAttemptAnswer
            {
                Correct = examAttemptAnswerDTO.Correct,
                Point = examAttemptAnswerDTO.Point
            });

        return (await _examAttemptDb.GradingExamAttempt(examAttempt)) ? StatusCode(201) : StatusCode(400);
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
            TotalPoint = pdfExamAttemptDTO.TotalPoint,
            StartedAt = pdfExamAttemptDTO.StartedAt.ToLocalTime(),
            Duration = duration,
            SubmittedAt = DateTime.Now,
            StudentAnswer = pdfExamAttemptDTO.StudentAnswer,
            PointBoard = pdfExamAttemptDTO.PointBoard,
            PdfExamCodeId = pdfExamAttemptDTO.PdfExamCodeId
        };

        return await _examAttemptDb.AddPdfExamAttempt(pdfExamAttempt) ? StatusCode(201, "Nộp bài thành công!") : StatusCode(400, "Nộp bài thất bại!");
    }

    [HttpPost("achievement/{studentId}")]
    public void SetAchivement(ulong studentId)
    {
        _achievementDb.DetechAchievement(1, studentId);
        _achievementDb.DetechAchievement(2, studentId);
        _achievementDb.DetechAchievement(3, studentId);
    }
}