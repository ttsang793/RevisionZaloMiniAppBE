using backend.Models;
using backend.Services;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public static class ConfigurationService
{
    public static void RegisterDb(this IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("ZALO_DB") ?? throw new Exception("Cannot connect...");
        service.AddDbContext<ZaloRevisionAppDbContext>(option => option.UseMySQL(connectionString));
    }

    public static void RegisterExternalServices(this IServiceCollection service, IConfiguration configuration)
    {
        // Cloudinary configuration
        var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL") ?? throw new Exception("Cloudinary URL not found.");
        var cloudinary = new Cloudinary(cloudinaryUrl);
        service.AddSingleton(cloudinary);

        // SMTP configuration
        var smtpSettings = new SmtpSettings
        {
            Server = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? throw new Exception("SMTP Server not found."),
            Port = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : throw new Exception("Invalid SMTP Port."),
            Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? throw new Exception("SMTP Username not found."),
            Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP Password not found.")
        };
        service.Configure<SmtpSettings>(options =>
        {
            options.Server = smtpSettings.Server;
            options.Port = smtpSettings.Port;
            options.Username = smtpSettings.Username;
            options.Password = smtpSettings.Password;
        });

        // Registering services
        service.AddScoped<SmtpService>();
        service.AddScoped<UploadService>();
    }
}
