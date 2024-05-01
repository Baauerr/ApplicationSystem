using Common.BannedToken;
using Common.Configuration;
using DocumentService.Configuration;
using DocumentService.DAL.Config;

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
