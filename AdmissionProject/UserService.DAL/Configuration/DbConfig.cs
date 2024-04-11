using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL.Repository;

namespace UserService.DAL.Configuration
{
    public static class DbConfig
    {
        public static void ConfigureUserDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(
                    builder.Services.BuildServiceProvider()
                    .GetRequiredService<IConfiguration>()
                    .GetConnectionString("UserDbConnection")
                    )
                );
        }
        public static void ConfigureRedisDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<RedisRepository>(
                new RedisRepository(
                    builder.Configuration.GetConnectionString("RedisDatabase"))
                );
        }
    }
}
