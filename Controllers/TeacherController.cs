using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TeacherController : Controller
{
    private readonly ILogger<TeacherController> _logger;
    private readonly UserDb _userDb;

    public TeacherController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _userDb = new UserDb(dbContext);
    }

    [HttpGet("{id}")]
    public async Task<TeacherDTO> GetTeacher(ulong id)
    {
        return await _userDb.GetTeacherDTOByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> AddTeacher(TeacherDTO t)
    {
        User user = new User
        {
            ZaloId = t.ZaloId,
            Name = t.Name,
            Avatar = t.Avatar,
            Role = "teacher"
        };

        Teacher teacher = new Teacher
        {
            SubjectId = t.SubjectId,
            Grades = t.Grades,
            Introduction = t.Introduction
        };

        return await _userDb.AddTeacher(user, teacher) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(TeacherDTO t, ulong id)
    {

        User user = new User
        {
            Id = id,
            Name = t.Name,
            Avatar = t.Avatar
        };

        Teacher teacher = new Teacher
        {
            Id = id,
            SubjectId = t.SubjectId,
            Grades = t.Grades,
            Introduction = t.Introduction
        };

        return await _userDb.UpdateTeacher(user, teacher, id) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(ulong id)
    {
        return await _userDb.NullifyTeacher(id) ? StatusCode(200) : StatusCode(400);
    }
}