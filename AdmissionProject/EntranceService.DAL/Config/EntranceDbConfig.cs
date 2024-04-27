using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EntranceService.DAL.Config
{
    public static class DbConfig
    {
        public static void ConfigureEntranceDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<EntranceDbContext>(options =>
                options.UseNpgsql(
                    builder.Services.BuildServiceProvider()
                    .GetRequiredService<IConfiguration>()
                    .GetConnectionString("EntranceDbConnection")
                    )
            );
        }
    }
}
