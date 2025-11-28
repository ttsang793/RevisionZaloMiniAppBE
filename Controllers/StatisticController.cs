using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/statistic")]
public class StatisticController
{
    private readonly ILogger<TeacherController> _logger;
    private readonly StatisticDb _statisticDb;

    public StatisticController(ILogger<TeacherController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _statisticDb = new StatisticDb(dbContext);
    }

    [HttpGet("student/{studentId}/{subjectId}")]
    public async Task<List<object>> GetStatistic(ulong studentId, string subjectId)
    {
        return
        [
            await _statisticDb.GetAvgPointMonthly(studentId, subjectId),
            await _statisticDb.GetCorrectRate(studentId, subjectId),
            await _statisticDb.GetBestResultExam(studentId, subjectId),
            await _statisticDb.GetWorstResultExam(studentId, subjectId)
        ];
    }
}
