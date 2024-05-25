using Exceptions;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.BL.Configuration;
using UserService.Configuration;
using UserService.DAL;
using UserService.DAL.Configuration;
using UserService.DAL.Entity;
using Common.Configuration;
using static Common.BannedToken.RedisConfig;
using Common.Enum;
using UserService.BL.Services;

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
builder.ConfigureHelpersServices();
builder.Services.AddListeners();
builder.Services.AddHttpClient();


var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<UserDbContext>();
dbContext?.Database.Migrate();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
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

        var adminPassword = builder.Configuration.GetValue<string>("ApiSettings:AdminPassword");
        var adminEmail = builder.Configuration.GetValue<string>("ApiSettings:AdminEmail");
        var adminFullName = builder.Configuration.GetValue<string>("ApiSettings:AdminFullName");

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User { UserName = adminEmail, Email = adminEmail, FullName = adminFullName };

            var result = await userManager.CreateAsync(adminUser, adminPassword); 

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Roles.ADMINISTRATOR.ToString());
            }
        }

    }
}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.ConfigureExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
