namespace backend.Services;

using backend.DTOs;
using backend.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

public class SmtpService(IOptions<SmtpSettings> smtpSettings)
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task<bool> SendEmailAsync(EmailDTO emailDTO)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zalo Revision App", _smtpSettings.Username));
            message.To.Add(new MailboxAddress("", emailDTO.ToEmail));
            message.Subject = emailDTO.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<html><body><i>{emailDTO.Body}</i><br /><br /><img src='https://res.cloudinary.com/dqxhmt5sp/image/upload/v1765116275/qr_fis5gs.png'/></body></html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
                await smtpClient.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }

            Console.WriteLine("--- Send email successfully! ---");
            return true;
        }

        catch (Exception e)
        {
            Console.WriteLine("--- Send email unsuccessfully! ---");
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> SendMultipleEmailsAsync(EmailDTO emailDTO, List<string> toEmails)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zalo Revision App", _smtpSettings.Username));
            //message.To.Add(new MailboxAddress("", _smtpSettings.Username));
            foreach (string toEmail in toEmails) message.Bcc.Add(new MailboxAddress("", toEmail));
            message.Subject = emailDTO.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<html><body><i>{emailDTO.Body}</i><br /><br /><img src='https://res.cloudinary.com/dqxhmt5sp/image/upload/v1765116275/qr_fis5gs.png'/></body></html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, false);
                await smtpClient.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }

            Console.WriteLine("--- Send email successfully! ---");
            return true;
        }

        catch (Exception e)
        {
            Console.WriteLine("--- Send email unsuccessfully! ---");
            Console.WriteLine(e);
            return false;
        }
    }
}