using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/exam")]
public class ExamController : Controller
{
    private readonly ILogger<QuestionController> _logger;
    private readonly ExamDb _examDb;
    private readonly ExamPartDb _examPartDb;
    private readonly ExamQuestionDb _examQuestionDb;

    public ExamController(ILogger<QuestionController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _examDb = new ExamDb(dbContext);
        _examPartDb = new ExamPartDb(dbContext);
        _examQuestionDb = new ExamQuestionDb(dbContext);
    }

    [HttpGet]
    public async Task<List<ExamReadDTO>> GetExamsAsync()
    {
        return await _examDb.GetExamsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ExamReadDTO> GetExamByIdAsync(ulong id)
    {
        return await _examDb.GetExamById(id);
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<List<ExamReadDTO>> GetExamsByTeacherAsync(ulong teacherId)
    {
        return await _examDb.GetExamsByTeacherAsync(teacherId);
    }

    [HttpPost]
    public async Task<IActionResult> AddExam(ExamInsertDTO examDTO)
    {
        Exam exam = new Exam
        {
            ExamType = examDTO.ExamType,
            DisplayType = examDTO.DisplayType,
            Title = examDTO.Title,
            Grade = examDTO.Grade,
            TimeLimit = (ushort)(examDTO.TimeLimit * 60),
            EarlyTurnIn = (ushort)(examDTO.EarlyTurnIn * 60),
            AllowShowScore = examDTO.AllowShowScore,
            AllowPartSwap = examDTO.AllowPartSwap,
            AllowQuestionSwap = examDTO.AllowQuestionSwap,
            AllowAnswerSwap = examDTO.AllowAnswerSwap,
            TeacherId = examDTO.TeacherId,
            SubjectId = examDTO.SubjectId
        };

        return await _examDb.AddExam(exam) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExam(ExamInsertDTO examDTO)
    {
        Exam exam = new Exam
        {
            Id = (ulong)examDTO.Id,
            ExamType = examDTO.ExamType,
            DisplayType = examDTO.DisplayType,
            Title = examDTO.Title,
            Grade = examDTO.Grade,
            TimeLimit = (ushort)(examDTO.TimeLimit * 60),
            EarlyTurnIn = (ushort)(examDTO.EarlyTurnIn * 60),
            AllowShowScore = examDTO.AllowShowScore,
            AllowPartSwap = examDTO.AllowPartSwap,
            AllowQuestionSwap = examDTO.AllowQuestionSwap,
            AllowAnswerSwap = examDTO.AllowAnswerSwap,
            TeacherId = examDTO.TeacherId,
            SubjectId = examDTO.SubjectId,
            Status = examDTO.Status
        };

        return await _examDb.UpdateExam(exam) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExam(ulong id)
    {
        return await _examDb.DeleteExam(id) ? StatusCode(200) : StatusCode(400);
    }

    [HttpPut("publish/{id}")]
    public async Task<IActionResult> PublishExam(ulong id)
    {
        return await _examDb.PublishExam(id) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("unpublish/{id}")]
    public async Task<IActionResult> UnpublishExam(ulong id)
    {
        return await _examDb.UnpublishExam(id) ? StatusCode(200) : StatusCode(400);
    }

    [HttpGet("question/{id}/detail")]
    public async Task<List<ExamPartDTO>> GetQuestionListForExam(ulong id)
    {
        return await _examPartDb.GetExamPartDetailsAsyncByExamId(id);
    }

    [HttpGet("question/{id}")]
    public async Task<List<ExamPart>> GetQuestionForExam(ulong id)
    {
        return await _examPartDb.GetExamPartsAsyncByExamId(id);
    }

    [HttpPost("question/{id}")]
    public async Task<IActionResult> UpdateQuestionForExam(ulong id, [FromBody] ExamQuestionsInsertDTO e)
    {
        if (e == null) return BadRequest("Invalid exam data.");
        bool success = true;

        try
        {
            await using var transaction = await _examDb.DbContext.Database.BeginTransactionAsync();

            // 1. Get all existing parts for the exam
            var existingParts = await _examPartDb.GetExamPartsAsyncByExamId(id);
            var existingPartTitles = existingParts.Select(p => p.PartTitle).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 2. Add new parts if needed
            for (byte i = 0; i < e.PartTitles.Count(); i++)
            {
                var title = e.PartTitles.ElementAt(i);
                if (!existingPartTitles.Contains(title))
                {
                    var newPart = new ExamPart
                    {
                        ExamId = id,
                        PartTitle = title
                    };

                    success &= await _examPartDb.AddExamPart(newPart);
                }
            }

            // 3. Refresh the list of parts (to get IDs)
            var updatedParts = await _examPartDb.GetExamPartsAsyncByExamId(id);

            // 4. Add questions
            foreach (var q in e.ExamQuestions)
            {
                var part = updatedParts.FirstOrDefault(p => p.PartTitle.Equals(q.PartTitle, StringComparison.OrdinalIgnoreCase));

                if (part == null)
                {
                    _logger.LogWarning($"No matching part found for question ${q.QuestionId} with part title {q.PartTitle}");
                    success = false;
                    break;
                }

                var newExamQuestion = new ExamQuestion
                {
                    ExamPartId = part.Id,
                    QuestionId = q.QuestionId,
                    OrderIndex = q.OrderIndex,
                    Point = q.Point
                };

                success &= await _examQuestionDb.AddExamQuestion(newExamQuestion);
            }

            if (success)
            {
                await _examDb.UpdateExam((ulong)e.ExamId);
                await transaction.CommitAsync();
                return StatusCode(200);
            }
            else
            {
                await transaction.RollbackAsync();
                return StatusCode(400);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating exam questions for exam {id}");
            return StatusCode(500, "Error updating exam questions: " + ex.Message);
        }
    }
}