using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public static class ConfigurationService
{
    public static void RegisterDb(this IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ZaloRevisionApp") ?? throw new Exception("Cannot connect...");
        service.AddDbContext<ZaloRevisionAppDbContext>(option => option.UseMySQL(connectionString));
    }
}
