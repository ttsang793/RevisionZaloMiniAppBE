namespace backend.Services;

using backend.DTOs;
using backend.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

public class StmpService(IOptions<StmpSettings> stmpSettings)
{
    private readonly StmpSettings _stmpSettings = stmpSettings.Value;

    public async Task<bool> SendEmailAsync(EmailDTO emailDTO)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zalo Revision App", _stmpSettings.Username));
            message.To.Add(new MailboxAddress("", emailDTO.ToEmail));
            message.Subject = emailDTO.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<html><body><i>{emailDTO.Body}</i></body></html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var stmpClient = new SmtpClient())
            {
                await stmpClient.ConnectAsync(_stmpSettings.Server, _stmpSettings.Port, false);
                await stmpClient.AuthenticateAsync(_stmpSettings.Username, _stmpSettings.Password);
                await stmpClient.SendAsync(message);
                await stmpClient.DisconnectAsync(true);
            }

            Console.WriteLine("--- Send email successfully! ---");
            return true;
        }

        catch
        {
            Console.WriteLine("--- Send email unsuccessfully! ---");
            return false;
        }
    }
}
