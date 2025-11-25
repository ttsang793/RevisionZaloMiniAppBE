using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("/api/student")]
public class StudentController : Controller
{
    private readonly ILogger<TeacherController> _logger;
    private readonly StudentDb _studentDb;

    public StudentController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _studentDb = new StudentDb(dbContext);
    }

    [HttpGet("{id}")]
    public async Task<User> GetStudentById(ulong id)
    {
        return await _studentDb.GetStudentByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(StudentDTO studentDTO)
    {
        User user = new User
        {
            ZaloId = studentDTO.ZaloId,
            Name = studentDTO.Name,
            Avatar = studentDTO.Avatar,
            Email = studentDTO.Email,
            Role = "HS"
        };

        Student student = new Student
        {
            Grade = studentDTO.Grade
        };

        return await _studentDb.AddStudent(user, student) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(StudentDTO studentDTO, ulong id)
    {
        User user = new User
        {
            Id = id,
            ZaloId = studentDTO.ZaloId,
            Name = studentDTO.Name,
            Avatar = studentDTO.Avatar,
            Email = studentDTO.Email
        };

        Student student = new Student
        {
            Grade = studentDTO.Grade
        };

        return await _studentDb.UpdateStudent(user, student) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(ulong id)
    {
        return await _studentDb.DeleteStudent(id) ? StatusCode(200) : StatusCode(400);
    }

    [HttpGet("attend/{studentId}")]
    public async Task<ICollection<DateTime>> GetAttendance(ulong studentId)
    {
        return await _studentDb.GetAttendance(studentId);
    }

    [HttpPut("attend/{studentId}")]
    public async Task<IActionResult> MarkAttendance(ulong studentId)
    {
        return await _studentDb.MarkAttendance(studentId) ? StatusCode(201) : StatusCode(400);
    }

    [HttpGet("favorite/{studentId}")]
    public async Task<List<ExamReadDTO>> GetFavorite(ulong studentId)
    {
        return await _studentDb.GetFavorite(studentId);
    }

    [HttpPut("favorite/{studentId}/{examId}")]
    public async Task<IActionResult> HandleFavorite(ulong studentId, ulong examId)
    {
        return await _studentDb.HandleFavorite(studentId, examId) ? StatusCode(200) : StatusCode(400);
    }

    [HttpGet("history/{studentId}")]
    public async Task<List<ExamReadDTO>> GetHistory(ulong studentId)
    {
        return await _studentDb.GetHistory(studentId);
    }

    [HttpPut("history/{studentId}/{examId}")]
    public async Task<IActionResult> HandleHistory(ulong studentId, ulong examId)
    {
        return await _studentDb.HandleHistory(studentId, examId) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("history/{id}")]
    public async Task<IActionResult> DeleteHistory(ulong id)
    {
        return await _studentDb.DeleteHistory(id) ? StatusCode(200) : StatusCode(404);
    }

    [HttpDelete("history/all/{studentId}")]
    public async Task<IActionResult> DeleteAllHistories(ulong studentId)
    {
        return await _studentDb.DeleteAllHistoriesByStudentId(studentId) ? StatusCode(200) : StatusCode(404);
    }

    [HttpPut("follow/{studentId}/{teacherId}")]
    public async Task<IActionResult> HandleFollowing(ulong studentId, ulong teacherId)
    {
        return await _studentDb.HandleFollowing(studentId, teacherId) ? StatusCode(200) : StatusCode(400);
    }
}