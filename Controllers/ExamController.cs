using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
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
    public async Task<Exam> GetExamByIdAsync(ulong id)
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
            SubjectId = examDTO.SubjectId,
            ApprovedBy = null,
            State = 1
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
            ApprovedBy = null,
            State = examDTO.State
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

    [HttpPost("question/${id}")]
    public async Task<IActionResult> UpdateQuestionForExam(ulong id, [FromBody] ExamQuestionsInsertDTO e)
    {
        if (e == null || e.ExamId == null || e.ExamId != id) return BadRequest("Invalid exam data.");
        bool success = true;
        var dbContext = new ZaloRevisionAppDbContext();

        try
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            // 1. Get all existing parts for the exam
            var existingParts = await _examPartDb.GetExamPartsAsyncByExamId(id);
            var existingPartTitles = existingParts.Select(p => p.PartTitle).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 2. Add new parts if needed
            foreach (var title in e.PartTitles)
            {
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
                // Map part title → part ID
                var part = updatedParts.FirstOrDefault(p => p.Id == q.Id);
                if (part == null) continue;

                q.ExamPartId = part.Id;

                success &= await _examQuestionDb.AddExamQuestion(q);
            }

            if (success)
            {
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
            await dbContext.Database.RollbackTransactionAsync();
            return StatusCode(500, "Error updating exam questions: " + ex.Message);
        }
    }
}