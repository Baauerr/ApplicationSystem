using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL.Repository;

namespace Common.BannedToken
{
    public static class RedisConfig
    {
        public static void ConfigureRedisDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<RedisRepository>(
                new RedisRepository(
                    builder.Configuration.GetConnectionString("RedisDatabase"))
              );
        }
    }
}
