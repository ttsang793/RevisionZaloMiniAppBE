using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/teacher")]
public class TeacherController : Controller
{
    private readonly ILogger<TeacherController> _logger;
    private readonly TeacherDb _teacherDb;

    public TeacherController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _teacherDb = new TeacherDb(dbContext);
    }

    [HttpGet("{id}")]
    public async Task<TeacherDTO> GetTeacherById(ulong id)
    {
        return await _teacherDb.GetTeacherDTOByIdAsync(id);
    }

    [HttpGet("{id}/subject")]
    public async Task<Subject> GetTeacherSubjectById(ulong id)
    {
        return await _teacherDb.GetTeacherSubjectByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddTeacher(TeacherDTO t)
    {
        User user = new User
        {
            ZaloId = t.ZaloId,
            Name = t.Name,
            Avatar = t.Avatar,
            Email = t.Email,
            Role = "GV"
        };

        Teacher teacher = new Teacher
        {
            SubjectId = t.SubjectId,
            Grades = t.Grades,
            Introduction = t.Introduction
        };

        return await _teacherDb.AddTeacher(user, teacher) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(TeacherDTO t, ulong id)
    {

        User user = new User
        {
            Id = id,
            Name = t.Name,
            Avatar = t.Avatar,
            Email = t.Email
        };

        Teacher teacher = new Teacher
        {
            Id = id,
            SubjectId = t.SubjectId,
            Grades = t.Grades,
            Introduction = t.Introduction
        };

        return await _teacherDb.UpdateTeacher(user, teacher) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(ulong id)
    {
        return await _teacherDb.NullifyTeacher(id) ? StatusCode(200) : StatusCode(400);
    }
}