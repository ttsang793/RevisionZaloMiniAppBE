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
    private readonly UserDb _studentDb;
    private readonly StudentProcessDb _studentProcessDb;

    public StudentController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _studentDb = new UserDb(dbContext);
        _studentProcessDb = new StudentProcessDb(dbContext);
    }

    [HttpGet("{id}")]
    public async Task<User> GetStudentById(ulong id)
    {
        return await _studentDb.GetUserByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(User user)
    {
        user.Role = "student";
        return await _studentDb.AddStudent(user) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(User user, ulong id)
    {
        return await _studentDb.UpdateUser(user, id) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(ulong id)
    {
        return await _studentDb.NullifyUser(id) ? StatusCode(200) : StatusCode(400);
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