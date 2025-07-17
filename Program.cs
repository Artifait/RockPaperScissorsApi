using Microsoft.EntityFrameworkCore;
using RockPaperScissorsApi.Data;
using RockPaperScissorsApi.Models;
using RockPaperScissorsApi.Services;
using System.IO;


// Получаем базовую папку, где запускается приложение
var basePath = Directory.GetCurrentDirectory();

// Собираем путь к БД корректно для любой ОС
var dbPath = Path.Combine(basePath, "app", "db", "app.db");

// Создаём директорию, если её нет
var dbDirectory = Path.GetDirectoryName(dbPath);
if (!Directory.Exists(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory!);
}

var builder = WebApplication.CreateBuilder(args);

// Добавляем политику CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(13080); // Порт API
});

// EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// DI
builder.Services.AddScoped<StatsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

var group = app.MapGroup("/api");

// Добавить результат
group.MapPost("/result", async (GameResult result, StatsService service) =>
{
    await service.AddResultAsync(result);
    return Results.Ok();
});

// Получить статистику по игроку
group.MapGet("/stats/{playerName}", async (string playerName, StatsService service) =>
{
    var stats = await service.GetStatsAsync(playerName);
    return stats is null ? Results.NotFound() : Results.Ok(stats);
});

// Лидерборд
group.MapGet("/leaderboard", async (StatsService service) =>
{
    var leaderboard = await service.GetLeaderboardAsync(10);
    return Results.Ok(leaderboard);
});

app.Run();
/*
docker run -d \
  --name rps-api \
  -p 13080:13080 \
  -v /app/db \
  --restart unless-stopped \
  artifait/rps-api:latest
*/
