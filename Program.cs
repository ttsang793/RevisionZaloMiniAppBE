using backend.Converters;
using backend.Services;
using backend.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "5273";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.Configure<StmpSettings>(builder.Configuration.GetSection("StmpSettings"));
builder.Services.AddScoped<StmpService>();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new QuestionDtoConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddDataProtection();
builder.Services.AddSession();
builder.Services.RegisterDb(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.UseSession();

app.UseCors();

app.Run();