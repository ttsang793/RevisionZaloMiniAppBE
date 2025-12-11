using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<StudentDTO> GetStudentById(ulong id)
    {
        return await _studentDb.GetStudentByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(StudentDTO studentDTO)
    {
        User user = new User
        {
            ZaloId = studentDTO.ZaloId,
            Name = studentDTO.Name!,
            Avatar = studentDTO.Avatar!,
            Email = studentDTO.Email,
            Role = "HS",
            Notification = [false, false, false]
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
            Name = studentDTO.Name!,
            Avatar = studentDTO.Avatar,
            Email = studentDTO.Email
        };

        Student student = new Student
        {
            Id = id,
            Grade = studentDTO.Grade
        };

        bool response = await _studentDb.UpdateStudent(user, student);
        Console.WriteLine(response ? "Thanh cong" : "That bai");

        return response ? StatusCode(200) : StatusCode(400);
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

    [HttpGet("favorite/{studentId}/{examId}")]
    public async Task<bool> IsFavorite(ulong studentId, ulong examId)
    {
        return await _studentDb.IsFavorite(studentId, examId);
    }

    [HttpPut("favorite/{studentId}/{examId}")]
    public async Task<IActionResult> HandleFavorite(ulong studentId, ulong examId)
    {
        return await _studentDb.HandleFavorite(studentId, examId) ? StatusCode(200) : StatusCode(400);
    }

    [HttpGet("history/{studentId}")]
    public async Task<List<StudentHistoryReadDTO>> GetHistory(ulong studentId)
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

    [HttpDelete("history/{studentId}/all")]
    public async Task<IActionResult> DeleteAllHistories(ulong studentId)
    {
        return await _studentDb.DeleteAllHistoriesByStudentId(studentId) ? StatusCode(200) : StatusCode(404);
    }

    [HttpPut("history/{studentId}/allow-save")]
    public async Task<IActionResult> UpdateAllowingSaveHistory(ulong studentId)
    {
        return await _studentDb.UpdateAllowingSaveHistory(studentId) ? StatusCode(200) : StatusCode(404);
    }

    [HttpGet("follow/{studentId}/{teacherId}")]
    public async Task<bool> IsFollowing(ulong studentId, ulong teacherId)
    {
        return await _studentDb.IsFollowing(studentId, teacherId);
    }

    [HttpPut("follow/{studentId}/{teacherId}")]
    public async Task<IActionResult> HandleFollowing(ulong studentId, ulong teacherId)
    {
        return await _studentDb.HandleFollowing(studentId, teacherId) ? StatusCode(200) : StatusCode(400);
    }

    [HttpGet("reminder/{studentId}")]
    public async Task<List<StudentReminder>> GetReminderById(ulong studentId)
    {
        return await _studentDb.GetReminderByIdAsync(studentId);
    }

    [HttpPost("reminder/{studentId}")]
    public async Task<IActionResult> AddReminder(ulong studentId)
    {
        return await _studentDb.AddReminderAsync(studentId) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("reminder/{id}")]
    public async Task<IActionResult> UpdateReminder(ulong id, StudentReminderDTO reminder)
    {
        return await _studentDb.UpdateReminderAsync(id, reminder) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("reminder/{id}")]
    public async Task<IActionResult> DeleteReminder(ulong id)
    {
        return await _studentDb.DeleteReminderAsync(id) ? StatusCode(200) : StatusCode(400);
    }
}