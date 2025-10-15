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
    private readonly TeacherDb _teacherDb;

    public TeacherController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _teacherDb = new TeacherDb(dbContext);
    }

    [HttpGet("{id}")]
    public async Task<TeacherDTO> GetTeacher(ulong id)
    {
        return await _teacherDb.GetTeacherByIdAsync(id);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTeacher(TeacherDTO t)
    {
        return await _teacherDb.UpdateTeacher(t) ? StatusCode(200) : StatusCode(400);
    }
}