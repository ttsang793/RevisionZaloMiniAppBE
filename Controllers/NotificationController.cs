using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController]
[Route("/api/notification")]
public class NotificationController : Controller
{
    private readonly ILogger<PdfExamCodeController> _logger;
    private readonly NotificationDb _notificationDb;

    public NotificationController(ILogger<PdfExamCodeController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _notificationDb = new NotificationDb(dbContext);
    }

    [HttpPut("student/{studentId}")]
    public async Task<IActionResult> UpdateStudentStatus(ulong studentId, string key, bool status)
    {
        return await _notificationDb.UpdateStudentStatus(studentId, key, status) ? StatusCode(200) : StatusCode(400);
    }

    [HttpPut("teacher/{teacherId}")]
    public async Task<IActionResult> UpdateTeacherStatus(ulong teacherId, string key, bool status)
    {
        return await _notificationDb.UpdateTeacherStatus(teacherId, key, status) ? StatusCode(200) : StatusCode(400);
    }
}