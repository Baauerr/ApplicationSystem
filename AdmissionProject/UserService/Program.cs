using Exceptions;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.BL.Configuration;
using UserService.Configuration;
using UserService.DAL;
using UserService.DAL.Configuration;
using UserService.DAL.Entity;
using static Common.BannedToken.RedisConfig;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.ConfigureUserDb();
builder.ConfigureRedisDb();
builder.ConfigureIdentity();
builder.ConfigureAccountServices();
builder.ConfigureSwagger();
builder.configureJWTAuth();
builder.Services.AddControllers();

var app = builder.Build();



using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<UserDbContext>();
dbContext?.Database.Migrate();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    foreach (var roleName in Enum.GetValues(typeof(Roles)))
    {
        var stringRoleName = roleName.ToString();

        if (stringRoleName == null)
        {
            throw new ArgumentNullException(nameof(roleName), "Роль равна null");
        }

        var role = await roleManager.FindByNameAsync(stringRoleName);
        if (role == null)
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(stringRoleName));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Ошибка при создании роли: {stringRoleName}");
            }

            role = await roleManager.FindByNameAsync(stringRoleName);
        }
        if (role == null || role.Name == null)
        {
            throw new NotFoundException($"Не получилось найти роль: {nameof(role)}");
        }
    }
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
