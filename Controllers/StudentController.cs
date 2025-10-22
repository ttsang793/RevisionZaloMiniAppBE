using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class StudentController : Controller
{
    private readonly ILogger<TeacherController> _logger;
    private readonly StudentDb _studentDb;
    private readonly StudentProcessDb _studentProcessDb;

    public StudentController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _studentDb = new StudentDb(dbContext);
        _studentProcessDb = new StudentProcessDb(dbContext);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(User user)
    {
        user.Role = "student";
        return await _studentDb.AddStudent(user) ? StatusCode(201) : StatusCode(400);
    }

    [HttpGet("favorite/{studentId}")]
    public async Task<List<ExamReadDTO>> GetFavorite(ulong studentId)
    {
        return await _studentProcessDb.GetFavorite(studentId);
    }

    [HttpPut("favorite")]
    public async Task<IActionResult> HandleFavorite(ulong examId, ulong studentId)
    {
        return await _studentProcessDb.HandleFavorite(examId, studentId) ? StatusCode(200) : StatusCode(400);
    }
}