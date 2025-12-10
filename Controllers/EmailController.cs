using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/email")]
public class EmailController : Controller
{
    private readonly ILogger<EmailController> _logger;
    private readonly SmtpService _stmpService;
    private readonly UserDb _userDb;
    private readonly TeacherDb _teacherDb;

    public EmailController(ILogger<EmailController> logger, SmtpService stmpService, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _stmpService = stmpService;
        _userDb = new UserDb(dbContext);
        _teacherDb = new TeacherDb(dbContext);
    }

    [HttpPost("comment")]
    public async Task NotifyWhenComment(EmailDTO emailDTO)
    {
        var user = await _userDb.GetUserByIdAsync(emailDTO.ToId!.Value);
        if (user == null || user.Notification[2] == false) return;

        emailDTO.ToEmail = user.Email;
        emailDTO.Subject = "Có bình luận mới";
        emailDTO.Body = $"Đề thi {emailDTO.TextValue} của bạn đã có bình luận mới. Hãy truy cập vào mini app để phản hồi nhé!";
        await _stmpService.SendEmailAsync(emailDTO);
    }

    [HttpPost("exam")]
    public async Task NotifyWhenNewExam(EmailDTO emailDTO)
    {
        List<string> emailList = await _teacherDb.GetTeacherFollowersNotifyByIdAsync(emailDTO.ToId!.Value);
        emailDTO.Subject = "Có đề mới từ giáo viên";
        emailDTO.Body = $"Giáo viên {emailDTO.TextValue} đã có đề thi mới. Em hãy truy cập vào mini app để xem nhé!";
        await _stmpService.SendMultipleEmailsAsync(emailDTO, emailList);
    }

    [HttpPost("turn-in")]
    public async Task NotifyWhenNewTurnIn(EmailDTO emailDTO)
    {
        var user = await _userDb.GetUserByIdAsync(emailDTO.ToId!.Value);
        if (user == null || user.Notification[1] == false) return;

        emailDTO.ToEmail = user.Email;
        emailDTO.Subject = "Có lượt làm bài mới";
        emailDTO.Body = $"Đề thi {emailDTO.TextValue} của bạn đã có lượt làm bài mới. Hãy truy cập vào mini app để chấm điểm và nhận xét nhé!";
        await _stmpService.SendEmailAsync(emailDTO);
    }

    [HttpPost("grading")]
    public async Task NotifyWhenFinishGrading(EmailDTO emailDTO)
    {
        var user = await _userDb.GetUserByIdAsync(emailDTO.ToId!.Value);
        if (user == null || user.Notification[2] == false) return;

        emailDTO.ToEmail = user.Email;
        emailDTO.Subject = "Đã có điểm bài làm";
        emailDTO.Body = $"Lượt làm bài của em ở đề {emailDTO.TextValue} đã có điểm số và nhận xét. Hãy truy cập vào mini app để xem kết quả nhé!";
        await _stmpService.SendEmailAsync(emailDTO);
    }
}
