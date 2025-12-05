using backend.Converters;
using backend.Services;
using backend.Models;
using System.Text.Json;
using CloudinaryDotNet;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Add Cloudinary
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
builder.Services.AddSingleton(cloudinary);

builder.Services.Configure<StmpSettings>(builder.Configuration.GetSection("StmpSettings"));
builder.Services.AddScoped<StmpService>();
builder.Services.AddScoped<UploadService>();

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