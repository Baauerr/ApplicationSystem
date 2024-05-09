using Common.Helpers;
using Common.Helpers.Impl;
using EntranceService.BL.Mapper;
using EntranceService.BL.Services;
using EntranceService.Common.Interface;
using Exceptions;
using UserService.Controllers.Policies.HITSBackEnd.Controllers.AttributeUsage;

namespace EntranceService.Configuration
{
    public static class ServiceConfig
    {
        public static void ConfigureEntranceServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IEntranceService, EntrancingService>();
            builder.Services.AddScoped<IEntrantService, EntrantService>();
            builder.Services.AddSingleton<ITokenHelper, TokenHelper>();
            builder.Services.AddTransient<ExceptionsHandler>();
            builder.Services.AddAutoMapper(typeof(EntranceMapper));
            builder.Services.AddScoped<TokenBlacklistFilterAttribute>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}


