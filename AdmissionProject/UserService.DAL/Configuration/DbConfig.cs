﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
