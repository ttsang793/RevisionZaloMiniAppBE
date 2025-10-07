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

    [HttpPost]
    public async Task<IActionResult> AddExam(ExamDTO examDTO)
    {
        Exam exam = new Exam
        {
            ExamType = examDTO.ExamType,
            DisplayType = examDTO.DisplayType,
            Title = examDTO.Title,
            Grade = examDTO.Grade,
            TimeLimit = (ushort)(examDTO.TimeLimit * 60),
            AllowTurnInTime = (ushort)(examDTO.AllowTurnInTime * 60),
            AllowShowScore = examDTO.AllowShowScore,
            AllowPartSwap = examDTO.AllowPartSwap,
            AllowQuestionSwap = examDTO.AllowQuestionSwap,
            AllowAnswerSwap = examDTO.AllowAnswerSwap,
            TeacherId = examDTO.TeacherId,
            SubjectId = examDTO.SubjectId,
            ApprovedBy = null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        return await _examDb.AddExam(exam) ? StatusCode(201) : StatusCode(400);
    }
}