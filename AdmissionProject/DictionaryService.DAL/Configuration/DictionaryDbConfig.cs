using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DictionaryService.DAL.Configuration
{
    public static class DictionaryDbConfig
    {
            public static void ConfigureDictionaryDb(this WebApplicationBuilder builder)
            {
                builder.Services.AddDbContext<DictionaryDbContext>(options =>
                    options.UseNpgsql(
                        builder.Services.BuildServiceProvider()
                        .GetRequiredService<IConfiguration>()
                        .GetConnectionString("DictionaryDbConnection")
                        )
                    );
            }
    }
}
