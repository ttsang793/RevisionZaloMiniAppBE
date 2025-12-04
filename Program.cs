using backend.Converters;
using backend.Services;
using backend.Models;
using System.Text.Json;
using CloudinaryDotNet;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use PORT environment variable
// var port = Environment.GetEnvironmentVariable("PORT") ?? "5273";
// builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add Cloudinary
var cloudinaryAccount = new Account(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);
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