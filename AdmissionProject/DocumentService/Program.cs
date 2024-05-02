using Common.BannedToken;
using Common.Configuration;
using DocumentService.Configuration;
using DocumentService.DAL;
using DocumentService.DAL.Config;
using Exceptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.ConfigureDocumentDb();
builder.ConfigureHelpersServices();
builder.ConfigureDocumentServices();
builder.ConfigureRedisDb();
builder.ConfigureSwagger();
builder.configureJWTAuth();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<DocumentDbContext>();
dbContext?.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
