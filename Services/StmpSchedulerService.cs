using backend.DTOs;
using backend.Services;
using Microsoft.EntityFrameworkCore;

// Background Service
public class EmailSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailSchedulerService> _logger;

    public EmailSchedulerService(IServiceProvider serviceProvider, ILogger<EmailSchedulerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;    
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Scheduler Service started");

        // Check every minute for emails that need to be sent
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSendScheduledEmailsAsync();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CheckAndSendScheduledEmailsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ZaloRevisionAppDbContext>();
        var smtpService = scope.ServiceProvider.GetRequiredService<SmtpService>();

        var now = DateTime.Now;
        var currentTime = now.TimeOfDay;
        var currentDay = (int)now.DayOfWeek;

        // Get all schedules
        var query = await (from sr in dbContext.StudentReminders
                           join u in dbContext.Users on sr.StudentId equals u.Id
                           where sr.IsActive
                           select new { sr.Date, sr.Hour, u.Email }
                          ).ToListAsync();
        
        List<string> emails = [];
        foreach (var row in query)
        {
            if (row.Date[currentDay] && row.Hour.Value.Hours == currentTime.Hours && row.Hour.Value.Minutes == currentTime.Minutes)
                emails.Add(row.Email);
        }

        if (emails.Count > 0)
        {
            var emailDTO = new EmailDTO
            {
                Subject = "Nhắc nhở học tập",
                Body = "Xin chào bạn! Đã tới giờ học bài rồi. Hãy truy cập vào mini app để ôn tập nhé!"
            };

            await smtpService.SendMultipleEmailsAsync(emailDTO, emails);
        }
    }
}