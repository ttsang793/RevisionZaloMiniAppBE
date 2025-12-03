using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/email")]
public class EmailController : Controller
{
    private readonly ILogger<EmailController> _logger;
    private readonly StmpService _stmpService;

    public EmailController(ILogger<EmailController> logger, StmpService stmpService)
    {
        _logger = logger;
        _stmpService = stmpService;
    }

    [HttpPost("new-response")]
    public async Task NotifyWhenResponse(EmailDTO emailDTO)
    {
        // To do code
    }

    [HttpPost("new-exam")]
    public async Task NotifyWhenNewExam(EmailDTO emailDTO)
    {
        emailDTO.Subject = "Có đề mới từ ...";
        emailDTO.Body = "Giáo viên ... đã có đề thi mới. Em hãy truy cập vào mini app để xem nhé!";
        await _stmpService.SendEmailAsync(emailDTO);
    }

    [HttpPost("new-turn-in")]
    public async Task NotifyWhenNewTurnIn(EmailDTO emailDTO)
    {
        emailDTO.Subject = "Có lượt làm bài mới";
        emailDTO.Body = "Đề thi ... của bạn đã có lượt làm bài mới. Hãy truy cập vào mini app để chấm điểm và nhận xét nhé!";
        await _stmpService.SendEmailAsync(emailDTO);
    }

    [HttpPost("new-point")]
    public async Task NotifyWhenFinishGrading(EmailDTO emailDTO)
    {
        emailDTO.Subject = "Đã có điểm bài làm";
        emailDTO.Body = "Lượt làm bài của em ở đề ... đã có điểm số và nhận xét. Hãy truy cập vào mini app để xem kết quả nhé!";
        await _stmpService.SendEmailAsync(emailDTO);
    }
}
