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
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IEntranceService, EntrancingService>();
            builder.Services.AddScoped<IEntrantService, EntrantService>();
            builder.Services.AddTransient<ExceptionsHandler>();
            builder.Services.AddScoped<TokenBlacklistFilterAttribute>();
        }
    }
}


