using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AchievementController : Controller
{
    private readonly ILogger<AchievementController> _logger;
    private readonly AchievementDb _achievementDb;

    public AchievementController(ILogger<AchievementController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _achievementDb = new AchievementDb(dbContext);
    }

    [HttpGet("")]
    public async Task<List<Achievement>> Get()
    {
        return await _achievementDb.GetAsync();
    }
}