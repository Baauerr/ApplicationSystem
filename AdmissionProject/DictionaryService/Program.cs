using DictionaryService.Configuration;
using DictionaryService.DAL;
using DictionaryService.DAL.Configuration;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using static Common.BannedToken.RedisConfig;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.ConfigureDictionaryDb();
builder.ConfigureDictionaryServices();
builder.configureJWTAuth();
builder.ConfigureRedisDb();
builder.Services.AddHttpClient();
builder.ConfigureSwagger();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<DictionaryDbContext>();
dbContext?.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
