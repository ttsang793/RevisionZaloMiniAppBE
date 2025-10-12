using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ExamController : Controller
{
    private readonly ILogger<QuestionController> _logger;
    private readonly ExamDb _examDb;

    public ExamController(ILogger<QuestionController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _examDb = new ExamDb(dbContext);
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
            State = (ushort)examDTO.State
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
}