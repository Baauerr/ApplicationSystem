using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentService.DAL.Config
{
    public static class DocumentDbConfig
    {
        public static void ConfigureDocumentDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DocumentDbContext>(options =>
                options.UseNpgsql(
                    builder.Services.BuildServiceProvider()
                    .GetRequiredService<IConfiguration>()
                    .GetConnectionString("DocumentDbConnection")
                    )
            );
        }
    }
}
