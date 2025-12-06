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
        service.Configure<StmpSettings>(configuration.GetSection("StmpSettings"));
        service.AddScoped<StmpService>();
        service.AddScoped<UploadService>();
    }
}
