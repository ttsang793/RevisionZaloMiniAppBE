using backend.Services;
using Microsoft.EntityFrameworkCore;
using dotenv.net;

namespace backend.Services;

public static class ConfigurationService
{
    public static void RegisterDb(this IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("ZALO_DB") ?? throw new Exception("Cannot connect...");
        service.AddDbContext<ZaloRevisionAppDbContext>(option => option.UseMySQL(connectionString));
    }
}
