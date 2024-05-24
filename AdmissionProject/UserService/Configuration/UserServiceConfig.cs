using Exceptions;
using UserService.BL.Configuration;
using UserService.BL.Mapper;
using UserService.BL.Services;
using UserService.Common.Interface;
using UserService.Controllers.Policies.HITSBackEnd.Controllers.AttributeUsage;
using UserService.DAL.Repository;

namespace UserService.Configuration
{
    public static class UserServiceConfig
    {
        public static void ConfigureAccountServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IQueueSender, QueueSender>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddSingleton<TokenProps>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddAutoMapper(typeof(UserMapper));
            builder.Services.AddTransient<ExceptionsHandler>();
            builder.Services.AddScoped<TokenBlacklistFilterAttribute>();
        }
    }
}
