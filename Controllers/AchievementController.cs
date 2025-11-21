using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/achievement")]
public class AchievementController : Controller
{
    private readonly ILogger<AchievementController> _logger;
    private readonly AchievementDb _achievementDb;

    public AchievementController(ILogger<AchievementController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _achievementDb = new AchievementDb(dbContext);
    }

    [HttpGet("{studentId}")]
    public List<Achievement> GetAchivements(ulong studentId)
    {
        return _achievementDb.GetAchievements(studentId);
    }
}