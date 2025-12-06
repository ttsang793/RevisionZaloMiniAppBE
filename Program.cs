using backend.Converters;
using backend.Services;
using dotenv.net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register db and services
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
builder.Services.RegisterDb(builder.Configuration);
builder.Services.RegisterExternalServices(builder.Configuration);

// Add localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Add Zalo Mini App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowZaloMiniApp", policy =>
        policy.WithOrigins("https://h5.zdn.vn")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});


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

app.UseCors("AllowLocalhost");
app.UseCors("AllowZaloMiniApp");

app.UseAuthorization();

app.MapControllers();

app.UseSession();

app.Run();