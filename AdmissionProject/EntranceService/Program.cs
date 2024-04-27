using Common.Configuration;
using EntranceService.Configuration;
using EntranceService.DAL;
using EntranceService.DAL.Config;
using Microsoft.EntityFrameworkCore;
using static Common.BannedToken.RedisConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.ConfigureEntranceServices();
builder.ConfigureEntranceDb();
builder.ConfigureRedisDb();
builder.Services.AddHttpClient();
builder.ConfigureSwagger();
builder.configureJWTAuth();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<EntranceDbContext>();
dbContext?.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();