using backend.Services;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController]
[Route("/api/user")]
public class UserController : Controller
{
    private readonly ILogger<TeacherController> _logger;
    private readonly UserDb _userDb;

    public UserController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _userDb = new TeacherDb(dbContext);
    }

    [HttpGet("{zaloId}")]
    public async Task<User> GetUserByZaloId(string zaloId)
    {
        return await _userDb.GetUserByZaloId(zaloId);
    }
}
