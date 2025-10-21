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

    public StudentController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _studentDb = new StudentDb(dbContext);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(User user)
    {
        user.Role = "student";
        return await _studentDb.AddStudent(user) ? StatusCode(201) : StatusCode(400);
    }
}